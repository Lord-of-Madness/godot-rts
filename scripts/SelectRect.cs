using Godot;
namespace RTS.UI
{
    public partial class SelectRect : Node2D
    {
        public Color color = new(0, 1, 0, 0.3f);//Probably alterable in settings eventually
        public Vector2 start;
        private Vector2 end;
        public bool dragging =false;
        public Vector2 Size { get => (end - start).Abs(); }
        public Rect2 Rect { get => new Rect2(start, end - start).Abs(); }
        public override void _Draw()
        {
            if (dragging)
            {
                DrawRect(new Rect2(start, end - start).Abs(), color);
            }
        }
        public void UpdateStats( Vector2 end)
        {
            this.end = end;
            QueueRedraw();
        }
    }
}