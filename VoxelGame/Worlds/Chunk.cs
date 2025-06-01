using SFML.Graphics;
using SFML.System;
using VoxelGame.Item;
using VoxelGame.Physics;
using VoxelGame.Resources;
using VoxelGame.Worlds.Tile;

namespace VoxelGame.Worlds
{
    public class Chunk : Transformable, Drawable
    {
        /// <summary>
        /// Chunk size
        /// </summary>
        public const int ChunkSize = 16;

        /// <summary>
        /// World (parent)
        /// </summary>
        private World _world;

        /// <summary>
        /// List of tiles
        /// </summary>
        private Tile.Tile[] _tiles;

        /// <summary>
        /// List of wall tiles
        /// </summary>
        private WallTile[] _tilesWall;

        /// <summary>
        /// List of bodies
        /// </summary>
        private List<AABB> _colliders;

        /// <summary>
        /// Chunk tile mesh dictionary
        /// </summary>
        private Dictionary<string, ChunkMesh> _chunkMeshs { get; }

        /// <summary>
        /// Chunk wall mesh dictionary
        /// </summary>
        private Dictionary<WallType, ChunkMesh> _chunkMeshsWall { get; }

        /// <summary>
        /// Local chunk position
        /// </summary>
        public int X, Y;

        /// <summary>
        /// Chunk identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Is the chunk complete (created)
        /// </summary>
        public bool IsComplate { get; set; }

        /// <summary>
        /// Tile light color
        /// </summary>
        public byte[,] LightColorTile { get; set; } = new byte[ChunkSize, ChunkSize];

        /// <summary>
        /// Wall light color
        /// </summary>
        public byte[,] LightColorWall { get; set; } = new byte[ChunkSize, ChunkSize];

        /// <summary>
        /// Chunk constructor
        /// </summary>
        /// <param name="world"> World (parent) </param>
        public Chunk(World world)
        {
            _world = world;

            _tiles = new Tile.Tile[ChunkSize * ChunkSize];
            _tilesWall = new WallTile[ChunkSize * ChunkSize];

            LightColorTile = new byte[ChunkSize, ChunkSize];
            LightColorWall = new byte[ChunkSize, ChunkSize];

            _colliders = new List<AABB>(ChunkSize * ChunkSize);
            _chunkMeshs = new Dictionary<string, ChunkMesh>();
            _chunkMeshsWall = new Dictionary<WallType, ChunkMesh>();
        }

        /// <summary>
        /// Is in chunk bounds?
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <returns> True if not out of chunk size and greater than -1 </returns>
        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < ChunkSize && y >= 0 && y < ChunkSize;
        }

        ///----------------------------Tiles---------------------------///

        /// <summary>
        /// Set tile in local coordinates, by tile type
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="type"> Tile type </param>
        /// <returns> True if tile is set </returns>
        public bool SetTile(int x, int y, TileType type, bool setWall = false, bool isPlayerSet = false)
        {
            if (!InBounds(x, y))
                return false;

            var upTile = GetTile(x, y - 1, true);
            var downTile = GetTile(x, y + 1, true);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);
            var wall = GetWall(x, y);

            if (type != TileType.None && (!IsComplate || !isPlayerSet))
            {
                var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, wall, new Vector2f(x, y));

                if (tile.Size.X > 1 || tile.Size.Y > 1)
                {
                    return SetMultipleTile(x, y, type, upTile, downTile, leftTile, rightTile, wall);
                }

                SetTileToMeshAndList(x, y, type, upTile, downTile, leftTile, rightTile, wall);

                if (setWall)
                    return SetWall(x, y, Tiles.TileTypeToWallType(type));
            }
            else if (type != TileType.None && IsComplate && isPlayerSet)
            {
                var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, wall, new Vector2f(x, y));

                if (tile.Size.X > 1 || tile.Size.Y > 1)
                {
                    return SetMultipleTile(x, y, type, upTile, downTile, leftTile, rightTile, wall);
                }

                if (upTile != null || downTile != null || leftTile != null || rightTile != null || wall != null)
                {
                    SetTileToMeshAndList(x, y, type, upTile, downTile, leftTile, rightTile, wall);
                }
                else
                    return false;
            }
            else if (type == TileType.None)
            {
                int index = x * ChunkSize + y;

                if (_tiles[index] != null)
                {
                    if (_chunkMeshs.ContainsKey(_tiles[index].TextureName))
                    {
                        SetTileToMesh(x, y, _tiles[index].TextureName, new Vertex[6]);
                    }
                    _world.DropItem(_tiles[index].DropItem, _tiles[index].GlobalPosition);

                    _tiles[index] = null;

                    if (upTile != null)
                        upTile!.DownTile = null;
                    if (downTile != null)
                        downTile!.UpTile = null;
                    if (leftTile != null)
                        leftTile!.RightTile = null;
                    if (rightTile != null)
                        rightTile!.LeftTile = null;
                }
            }

            if (IsComplate)
                GenerateColliders();

            return true;
        }

        /// <summary>
        /// Set tile in local coordinates, by world coordinates and tile type
        /// </summary>
        /// <param name="position"> Position </param>
        /// <param name="tile"> Tile </param>
        /// <returns> True if tile is set </returns>
        public bool SetTileByWorldPosition(Vector2f position, TileType type, bool setWall = false, bool isPlayerSet = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return SetTile(x, y, type, setWall, isPlayerSet);
        }

        public Tile.Tile SetTileToMeshAndList(int x, int y, TileType type, Tile.Tile? upTile, Tile.Tile? downTile,
            Tile.Tile? leftTile, Tile.Tile? rightTile, WallTile? wall)
        {
            if(x >= ChunkSize)
            {
                return _world.GetChunk(X + 1, Y)?.SetTileToMeshAndList(ChunkSize - x, y, type, upTile, downTile, leftTile, rightTile, wall)!;
            }
            else if(x < 0)
            {
                return _world.GetChunk(X - 1, Y)?.SetTileToMeshAndList(ChunkSize + x, y, type, upTile, downTile, leftTile, rightTile, wall)!;
            }
            else if (y >= ChunkSize)
            {
                return _world.GetChunk(X, Y + 1)?.SetTileToMeshAndList(x, ChunkSize - y, type, upTile, downTile, leftTile, rightTile, wall)!;
            }
            else if (y < 0)
            {
                return _world.GetChunk(X, Y - 1)?.SetTileToMeshAndList(x, ChunkSize + y, type, upTile, downTile, leftTile, rightTile, wall)!;
            }

            int index = x * ChunkSize + y;

            if (index > _tiles.Length)
                return null;

            if (_tiles[index] != null)
            {
                if (_chunkMeshs.ContainsKey(_tiles[index].TextureName))
                {
                    SetTileToMesh(x, y, _tiles[index].TextureName, new Vertex[6]);
                }
            }

            var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, wall, new Vector2f(x, y));
            tile.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

            SetTileToMesh(x, y, tile.TextureName, tile.GetVertices());


            _tiles[index] = tile;

            return tile;
        }

        public Tile.Tile SetTileToMeshAndList(int x, int y, TileType type)
        {
            var upTile = GetTile(x, y - 1, true);
            var downTile = GetTile(x, y + 1, true);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);
            var wall = GetWall(x, y);

            return SetTileToMeshAndList(x, y, type, upTile, downTile, leftTile, rightTile, wall);
        }

        /// <summary>
        /// Set a multi-tile object (e.g. workbench)
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="type"> Multi-tile type </param>
        /// <returns></returns>
        public bool SetMultipleTile(int startX, int startY, TileType type, Tile.Tile? upTile, Tile.Tile? downTile,
            Tile.Tile? leftTile, Tile.Tile? rightTile, WallTile? wall)
        {
            var tile = Tiles.GetTile(type, this, upTile, downTile, leftTile, rightTile, wall, new Vector2f(startX, startY));

            if (tile == null)
                return false;

            bool setRight = true;
            bool setLeft = true;

            for (int x = 0; x < tile.Size.X; x++)
            {
                for (int y = 0; y < tile.Size.Y; y++)
                {
                    if (GetTile(x + startX, startY - y, true) != null)
                    {
                        setRight = false;
                        break;
                    }

                    if (GetTile(x + startX, startY + 1, true) == null || !GetTile(x + startX, startY + 1, true)!.IsSolid)
                    {
                        setRight = false;
                        break;
                    }
                }

                if (!setRight)
                    break;
            }

            Tile.Tile? baseTile = null;

            if (setRight)
            {
                for (int x = 0; x < tile.Size.X; x++)
                {
                    for (int y = 0; y < tile.Size.Y; y++)
                    {
                        if (baseTile == null)
                            baseTile = SetTileToMeshAndList(x + startX, startY - y, type);
                        else
                        {
                            var newTile = SetTileToMeshAndList(x + startX, startY - y, type);
                            newTile.PerentTile = baseTile;
                            baseTile.ChildTiles.Add(newTile);
                        }
                    }
                }

                return true;
            }
            else
            {
                for (int x = 0; x < tile.Size.X; x++)
                {
                    for (int y = 0; y < tile.Size.Y; y++)
                    {
                        if (GetTile(startX - x, startY - y, true) != null)
                        {
                            setLeft = false;
                            break;
                        }

                        if (GetTile(startX - x, startY + 1, true) == null || !GetTile(startX - x, startY + 1, true)!.IsSolid)
                        {
                            setLeft = false;
                            break;
                        }
                    }

                    if (!setLeft)
                        break;
                }

                if (setLeft)
                {
                    for (int x = 0; x < tile.Size.X; x++)
                    {
                        for (int y = 0; y < tile.Size.Y; y++)
                        {
                            if (baseTile == null)
                                baseTile = SetTileToMeshAndList(startX - x, startY - y, type);
                            else
                            {
                                var newTile = SetTileToMeshAndList(startX - x, startY - y, type);
                                newTile.PerentTile = baseTile;
                                baseTile.ChildTiles.Add(newTile);
                            }
                        }
                    }
                    return true;

                }
                else
                {
                    for (int x = -(int)tile.Size.X / 2; x < tile.Size.X / 2; x++)
                    {
                        for (int y = 0; y < tile.Size.Y; y++)
                        {
                            if (GetTile(startX + x, startY - y, true) != null)
                            {
                                return false;
                            }

                            if (GetTile(startX + x, startY + 1, true) == null || !GetTile(startX + x, startY + 1, true)!.IsSolid)
                            {
                                return false;
                            }
                        }

                        if (!setLeft)
                            break;
                    }

                    for (int x = -(int)tile.Size.X / 2; x < tile.Size.X / 2; x++)
                    {
                        for (int y = 0; y < tile.Size.Y; y++)
                        {
                            if (baseTile == null)
                                baseTile = SetTileToMeshAndList(startX + x, startY - y, type);
                            else
                            {
                                var newTile = SetTileToMeshAndList(startX + x, startY - y, type);
                                newTile.PerentTile = baseTile;
                                baseTile.ChildTiles.Add(newTile);
                            }
                        }
                    }

                    return true;
                }
            }
        }

        public bool BreakingWorkbench(int x, int y)
        {
            var tile = GetTile(x, y);
            var leftTile = GetTile(x - 1, y, true);
            var rightTile = GetTile(x + 1, y, true);

            if (tile != null && tile is TileWorkbench)
            {
                SetTile(x, y, TileType.None);
                if (leftTile != null && leftTile is TileWorkbench)
                {
                    var upTile = GetTile(x - 1, y - 1, true);
                    var downTile = GetTile(x - 1, y + 1, true);
                    leftTile = GetTile(x - 2, y, true);
                    rightTile = GetTile(x, y, true);

                    int index = (x - 1) * ChunkSize + y;

                    if (_tiles[index] != null)
                    {
                        if (_chunkMeshs.ContainsKey(_tiles[index].TextureName))
                        {
                            SetTileToMesh(x - 1, y, _tiles[index].TextureName, new Vertex[6]);
                        }

                        _tiles[index] = null;

                        if (upTile != null)
                            upTile!.DownTile = null;
                        if (downTile != null)
                            downTile!.UpTile = null;
                        if (leftTile != null)
                            leftTile!.RightTile = null;
                        if (rightTile != null)
                            rightTile!.LeftTile = null;

                        return true;
                    }
                }
                else if (rightTile != null && rightTile is TileWorkbench)
                {
                    var upTile = GetTile(x + 1, y - 1, true);
                    var downTile = GetTile(x + 1, y + 1, true);
                    leftTile = GetTile(x, y, true);
                    rightTile = GetTile(x + 2, y, true);

                    int index = (x + 1) * ChunkSize + y;

                    if (_tiles[index] != null)
                    {
                        if (_chunkMeshs.ContainsKey(_tiles[index].TextureName))
                        {
                            SetTileToMesh(x + 1, y, _tiles[index].TextureName, new Vertex[6]);
                        }

                        _tiles[index] = null;

                        if (upTile != null)
                            upTile!.DownTile = null;
                        if (downTile != null)
                            downTile!.UpTile = null;
                        if (leftTile != null)
                            leftTile!.RightTile = null;
                        if (rightTile != null)
                            rightTile!.LeftTile = null;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Set tile in local mesh coordinates, by tile type
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="vertices"></param>
        public void SetTileToMesh(int x, int y, string textureName, Vertex[] vertices)
        {
            if (_chunkMeshs.ContainsKey(textureName))
            {
                _chunkMeshs[textureName].SetToMesh(x, y, vertices);
            }
            else
            {
                var chunkMesh = new ChunkMesh(this);
                chunkMesh.SetToMesh(x, y, vertices);

                _chunkMeshs.Add(textureName, chunkMesh);
            }
        }

        /// <summary>
        /// Get tile by local coordinates
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <returns> Tile, null if x,y are out of chunk bounds or tile is null </returns>
        public Tile.Tile? GetTile(int x, int y, bool getOtherChunk = false)
        {
            if (!InBounds(x, y) && !getOtherChunk)
            {
                return null;
            }
            else if (getOtherChunk)
            {
                if (x < 0)
                {
                    return _world.GetChunk(X - 1, Y)?.GetTile(ChunkSize + x, y);
                }
                else if (x >= ChunkSize)
                {
                    return _world.GetChunk(X + 1, Y)?.GetTile(x - ChunkSize, y);
                }
                else if (y < 0)
                {
                    return _world.GetChunk(X, Y - 1)?.GetTile(x, ChunkSize + y);
                }
                else if (y >= ChunkSize)
                {
                    return _world.GetChunk(X, Y + 1)?.GetTile(x, y - ChunkSize);
                }
            }


            return _tiles[x * ChunkSize + y];
        }

        /// <summary>
        /// Get tile by world coordinates
        /// </summary>
        /// <param name="position"> Position </param>
        /// <returns> Tile, null if x,y are out of chunk bounds or tile is null </returns>
        public Tile.Tile? GetTileByWorldPosition(Vector2f position, bool getOtherChunk = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return GetTile(x, y, getOtherChunk);
        }

        /// <summary>
        /// Get tile by tile type
        /// </summary>
        /// <param name="type"> Tile type </param>
        /// <returns></returns>
        public Tile.Tile? GetTileByType(TileType type)
        {
            return _tiles.FirstOrDefault(t => t != null && t.Type == type);
        }

        /// <summary>
        /// Get list of tiles by tile type
        /// </summary>
        /// <param name="type"> Tile type </param>
        /// <returns></returns>
        public List<Tile.Tile> GetTilesByType(TileType type)
        {
            return _tiles.Where(t => t != null && t.Type == type).ToList();
        }

        /// <summary>
        /// Break tile by local coordinates
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <param name="type"> Item type used to break </param>
        /// <param name="itemPower"> Item power </param>
        /// <param name="damage"> Tile damage </param>
        /// <returns></returns>
        public bool BreakingTile(int x, int y, ItemType type, float itemPower, float damage)
        {
            var tile = GetTile(x, y);

            if (tile != null)
            {
                if (tile.Breaking(type, itemPower, damage))
                {
                    if (tile.IsTree)
                    {
                        ChoppingTree(tile, x, y);
                    }

                    switch (tile.Type)
                    {
                        case TileType.Workbench:
                            return BreakingWorkbench(x, y);
                    }

                    return SetTile(x, y, TileType.None);
                }
            }

            return false;
        }

        /// <summary>
        /// Break tile by world coordinates
        /// </summary>
        /// <param name="position"> Tile position </param>
        /// <param name="type"> Item type used to break </param>
        /// <param name="itemPower"> Item power </param>
        /// <param name="damage"> Tile damage </param>
        /// <returns></returns>
        public bool BreakingTileByWorldPosition(Vector2f position, ItemType type, float itemPower, float damage)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return BreakingTile(x, y, type, itemPower, damage);
        }

        /// <summary>
        /// Update tile view by local coordinates
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <param name="vertices"> Vertex array </param>
        public void UpdateTileView(int x, int y, Vertex[] vertices)
        {
            var tile = GetTile(x, y);

            if (tile == null)
                return;

            _chunkMeshs[tile.TextureName].UpdateViewMesh(x, y, vertices);
        }

        /// <summary>
        /// Update tile view by world coordinates
        /// </summary>
        /// <param name="position"> Tile position </param>
        /// <param name="vertices"> Vertex array </param>
        public void UpdateTileViewByWorldPosition(Vector2f position, Vertex[] vertices)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            UpdateTileView(x, y, vertices);
        }

        /// <summary>
        /// Update tile color by local coordinates
        /// </summary>
        /// <param name="color"> Color </param>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        public bool UpdateTileColor(Color color, int x, int y, bool useOtherChunk = false)
        {
            var tile = GetTile(x, y, useOtherChunk);

            if (tile != null && tile.PerentChunk != this)
            {
                tile.PerentChunk.UpdateTileColor(color, (int)tile.LocalPosition.X, (int)tile.LocalPosition.Y, useOtherChunk);

                return true;
            }
            else if(tile != null && tile.PerentChunk == this)
            {
                _chunkMeshs[tile.TextureName].UpdateTileColor(color, (int)tile.LocalPosition.X, (int)tile.LocalPosition.Y);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Get tile color by local coordinates
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <returns></returns>
        public Color GetTileColor(int x, int y, bool useOtherChunk = false)
        {
            var tile = GetTile(x, y, useOtherChunk);

            if (tile != null && tile.PerentChunk != this)
            {
                return tile.PerentChunk.GetTileColor((int)tile.LocalPosition.X, (int)tile.LocalPosition.Y, useOtherChunk);
            }
            else if(tile != null && tile.PerentChunk == this)
            {
                return _chunkMeshs[tile.TextureName].GetTileColor((int)tile.LocalPosition.X, (int)tile.LocalPosition.Y);
            }

            return Color.Black;
        }


        ///------------------------------Walls-------------------------///

        /// <summary>
        /// Set wall in local coordinates, by wall type
        /// </summary>
        /// <param name="type"> Wall type </param>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        public bool SetWall(int x, int y, WallType type, bool isPlayerSet = false)
        {
            if (!InBounds(x, y))
                return false;

            var upWall = GetWall(x, y - 1, true);
            var downWall = GetWall(x, y + 1, true);
            var leftWall = GetWall(x - 1, y, true);
            var rightWall = GetWall(x + 1, y, true);

            if (type != WallType.None && !isPlayerSet)
            {
                SetWallToMeshAndList(x, y, type, upWall, downWall, leftWall, rightWall);
            }
            else if (type != WallType.None && isPlayerSet)
            {
                var upTile = GetTile(x, y - 1, true);
                var downTile = GetTile(x, y + 1, true);
                var leftTile = GetTile(x - 1, y, true);
                var rightTile = GetTile(x + 1, y, true);
                var tile = GetTile(x, y, true);

                if (upWall != null || downWall != null || leftWall != null || rightWall != null
                    || upTile != null || downTile != null || leftTile != null || rightTile != null || tile != null)
                {
                    SetWallToMeshAndList(x, y, type, upWall, downWall, leftWall, rightWall);
                }
                else
                    return false;
            }
            else if (type == WallType.None)
            {
                int index = x * ChunkSize + y;

                if (_tilesWall[index] != null)
                {
                    if (_chunkMeshsWall.ContainsKey(_tilesWall[index].Type))
                    {
                        SetWallToMesh(x, y, _tilesWall[index].Type, new Vertex[6]);
                    }

                    _world.DropItem(_tilesWall[index].DropItem, _tilesWall[index].GlobalPosition);

                    _tilesWall[index] = null;

                    if (upWall != null)
                        upWall!.DownWall = null;
                    if (downWall != null)
                        downWall!.UpWall = null;
                    if (leftWall != null)
                        leftWall!.RightWall = null;
                    if (rightWall != null)
                        rightWall!.LeftWall = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Set wall in local coordinates, by world coordinates and wall type
        /// </summary>
        /// <param name="position"> Position in world coordinates </param>
        /// <param name="type"> Wall type </param>
        /// <returns></returns>
        public bool SetWallByWorldPosition(Vector2f position, WallType type, bool isPlayerSet = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return SetWall(x, y, type, isPlayerSet);
        }

        /// <summary>
        /// Set wall in local mesh coordinates, by wall type
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="type"> Wall type </param>
        /// <param name="vertices"> Vertices </param>
        public void SetWallToMesh(int x, int y, WallType type, Vertex[] vertices)
        {
            if (_chunkMeshsWall.ContainsKey(type))
            {
                _chunkMeshsWall[type].SetToMesh(x, y, vertices);
            }
            else
            {
                var chunkMesh = new ChunkMesh(this);
                chunkMesh.SetToMesh(x, y, vertices);

                _chunkMeshsWall.Add(type, chunkMesh);
            }
        }

        public void SetWallToMeshAndList(int x, int y, WallType type, WallTile? upWall, WallTile? downWall, WallTile? leftWall, WallTile? rightWall)
        {
            var wall = Tiles.GetWall(type, this, upWall, downWall, leftWall, rightWall, new Vector2f(x, y));
            wall.GlobalPosition = new Vector2f(x, y) * Tile.Tile.TileSize + Position;

            int index = x * ChunkSize + y;

            _tilesWall[index] = wall;

            SetWallToMesh(x, y, type, wall.GetVertices());
        }

        /// <summary>
        /// Get wall by local coordinates
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="getOtherChunk"> Check neighboring chunks? </param>
        /// <returns></returns>
        public WallTile? GetWall(int x, int y, bool getOtherChunk = false)
        {
            if (!InBounds(x, y) && !getOtherChunk)
            {
                return null;
            }
            else if (getOtherChunk)
            {
                if (x < 0)
                {
                    return _world.GetChunk(X - 1, Y)?.GetWall(ChunkSize + x, y);
                }
                else if (x >= ChunkSize)
                {
                    return _world.GetChunk(X + 1, Y)?.GetWall(x - ChunkSize, y);
                }
                else if (y < 0)
                {
                    return _world.GetChunk(X, Y - 1)?.GetWall(x, ChunkSize + y);
                }
                else if (y >= ChunkSize)
                {
                    return _world.GetChunk(X, Y + 1)?.GetWall(x, y - ChunkSize);
                }
            }


            return _tilesWall[x * ChunkSize + y];
        }

        /// <summary>
        /// Get wall by world coordinates
        /// </summary>
        /// <param name="position"> Position in world coordinates </param>
        /// <param name="getOtherChunk"> Check neighboring chunks? </param>
        /// <returns></returns>
        public WallTile? GetWallByWorldPosition(Vector2f position, bool getOtherChunk = false)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return GetWall(x, y, getOtherChunk);
        }

        /// <summary>
        /// Break wall by local coordinates
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <param name="type"> Item type used to break </param>
        /// <param name="itemPower"> Item power </param>
        /// <param name="damage"> Wall damage </param>
        /// <returns></returns>
        public bool BreakingWall(int x, int y, ItemType type, float itemPower, float damage)
        {
            var tile = GetWall(x, y);

            if (tile != null)
            {
                if (tile.Breaking(type, itemPower, damage))
                {
                    return SetWall(x, y, WallType.None);
                }
            }

            return false;
        }

        /// <summary>
        /// Break wall by world coordinates
        /// </summary>
        /// <param name="position"> Wall position </param>
        /// <param name="type"> Item type used to break </param>
        /// <param name="itemPower"> Item power </param>
        /// <param name="damage"> Wall damage </param>
        /// <returns></returns>
        public bool BreakingWallByWorldPosition(Vector2f position, ItemType type, float itemPower, float damage)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            return BreakingWall(x, y, type, itemPower, damage);
        }

        /// <summary>
        /// Update wall view by local coordinates
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="vertices"> Vertex array </param>
        public void UpdateWallView(int x, int y, Vertex[] vertices)
        {
            var tile = GetWall(x, y);

            if (tile == null)
                return;

            _chunkMeshsWall[tile.Type].UpdateViewMesh(x, y, vertices);
        }

        /// <summary>
        /// Update wall view by world coordinates
        /// </summary>
        /// <param name="position"> Wall position in world coordinates </param>
        /// <param name="vertices"> Vertex array </param>
        public void UpdateWallViewByWorldPosition(Vector2f position, Vertex[] vertices)
        {
            int x = (int)((position.X - Position.X) / Tile.Tile.TileSize);
            int y = (int)((position.Y - Position.Y) / Tile.Tile.TileSize);

            UpdateWallView(x, y, vertices);
        }

        /// <summary>
        /// Update wall color by local coordinates
        /// </summary>
        /// <param name="color"> Color </param>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        public bool UpdateWallColor(Color color, int x, int y, bool useOtherChunk = false)
        {
            var wall = GetWall(x, y, useOtherChunk);

            if (wall != null && wall.PerentChunk != this)
            {
                wall.PerentChunk.UpdateWallColor(color, (int)wall.LocalPosition.X, (int)wall.LocalPosition.Y, useOtherChunk);

                return true;
            }
            else if (wall != null && wall.PerentChunk == this)
            {
                _chunkMeshsWall[wall.Type].UpdateTileColor(color, (int)wall.LocalPosition.X, (int)wall.LocalPosition.Y);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get wall color by local coordinates
        /// </summary>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <returns></returns>
        public Color GetWallColor(int x, int y, bool useOtherChunk = false)
        {
            var wall = GetWall(x, y, useOtherChunk);

            if (wall != null && wall.PerentChunk != this)
            {
                return wall.PerentChunk.GetWallColor((int)wall.LocalPosition.X, (int)wall.LocalPosition.Y, useOtherChunk);
            }
            else if (wall != null && wall.PerentChunk == this)
            {
                return _chunkMeshsWall[wall.Type].GetTileColor((int)wall.LocalPosition.X, (int)wall.LocalPosition.Y);
            }

            return Color.Black;
        }

        /// <summary>
        /// Chop tree
        /// </summary>
        /// <param name="treeTile"> Tree tile </param>
        /// <param name="x"> X position </param>
        /// <param name="startY"> Y position </param>
        /// <returns></returns>
        public bool ChoppingTree(Tile.Tile treeTile, int x, int startY)
        {
            for (int y = 0; treeTile != null && (treeTile.IsTree || treeTile.IsTreeTop); y--)
            {
                if (y != 0)
                    treeTile = GetTile(x, startY + y, true)!;

                if (treeTile != null)
                {
                    if (!SetTile(x, startY + y, TileType.None))
                    {
                        _world.GetChunk(X, Y - 1)?.SetTile(x, ChunkSize + (startY + y), TileType.None);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Generate chunk colliders
        /// </summary>
        public void GenerateColliders()
        {
            _colliders.Clear();
            Vector2f[] vertices = new Vector2f[4];

            for (int i = 0; i < 4; i++)
                vertices[i] = new Vector2f(-1, -1);

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    var tile = GetTile(x, y);
                    var upTile = GetTile(x, y + 1);
                    var downTile = GetTile(x, y - 1);
                    var leftTile = GetTile(x - 1, y);
                    var rightTile = GetTile(x + 1, y);

                    if (tile == null || !tile.IsSolid)
                        continue;

                    if ((upTile == null || !upTile.IsSolid) || (downTile == null || !downTile.IsSolid) || (leftTile == null || !leftTile.IsSolid) || (rightTile == null || !rightTile.IsSolid))
                    {
                        _colliders.Add(new AABB(tile.GlobalPosition, tile.GlobalPosition + new Vector2f(Tile.Tile.TileSize, Tile.Tile.TileSize)));
                    }
                }
            }
        }

        /// <summary>
        /// Update chunk
        /// </summary>
        /// <param name="deltaTime"> Frame time </param>
        public void Update(float deltaTime)
        {
            for(int i = 0; i < _tiles.Length; i++)
            {
                if(_tiles[i] == null)
                    continue;

                var tile = _tiles[i];

                var tileUp = tile.UpTile;
                var tileDown = tile.DownTile;
                var tileLeft = tile.LeftTile;
                var tileRight = tile.RightTile;

                var tileUpLeft = GetTile((int)tile.LocalPosition.X - 1, (int)tile.LocalPosition.Y - 1, true);
                var tileUpRight = GetTile((int)tile.LocalPosition.X + 1, (int)tile.LocalPosition.Y - 1, true);

                if (tile.Type == TileType.Ground)
                {
                    var randDouble = Random.Shared.NextDouble();

                    if (randDouble < 0.003) // 0.3% что в этом кадре появится трава
                    {
                        if ((tileUp == null || !tileUp.IsSolid) && ((tileLeft != null && tileLeft.Type == TileType.Grass) || (tileRight != null && tileRight.Type == TileType.Grass) || (tileUpLeft != null && tileUpLeft.Type == TileType.Grass) || (tileUpRight != null && tileUpRight.Type == TileType.Grass)))
                        {
                            SetTile((int)tile.LocalPosition.X, (int)tile.LocalPosition.Y, TileType.Grass);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw chunk walls
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void DrawWall(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var chunkMesh in _chunkMeshsWall)
            {
                states.Texture = TextureManager.GetTexture(chunkMesh.Key.ToString());

                target.Draw(chunkMesh.Value.GetChunkMesh(), PrimitiveType.Triangles, states);
            }
        }

        /// <summary>
        /// Draw chunk tiles
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void DrawTile(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            foreach (var chunkMesh in _chunkMeshs)
            {
                states.Texture = TextureManager.GetTexture(chunkMesh.Key);

                target.Draw(chunkMesh.Value.GetChunkMesh(), PrimitiveType.Triangles, states);
            }
        }

        /// <summary>
        /// Get chunk AABB
        /// </summary>
        /// <returns> AABB </returns>
        public AABB GetAABB()
        {
            return new AABB(Position.X, Position.Y, ChunkSize * Tile.Tile.TileSize, ChunkSize * Tile.Tile.TileSize);
        }

        /// <summary>
        /// Get list of colliders
        /// </summary>
        public List<AABB> GetColliders()
        {
            return _colliders;
        }

        /// <summary>
        /// Implement Drawable interface
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
        }

        /// <summary>
        /// Возвращает родительский объект мира, к которому принадлежит данный чанк.
        /// </summary>
        /// <returns>Объект мира, связанный с данным чанком.</returns>
        public World GetWorld() => _world;
    }
}
