namespace VoxelGame.UI
{
    public enum WindowStyles
    {
        Default,
        Clouseble
    }


    public struct UIWindowStyles
    {
        public WindowStyles Styles { get; set; }

        public UIWindowStyles(WindowStyles styles)
        {
            Styles = styles;
        }

        public static UIWindowStyles Default { get => new UIWindowStyles(WindowStyles.Default); }
    }
}
