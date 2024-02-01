namespace Entity.Module
{
    public interface IShot
    {
        public Entity Target { get; set; }
        
        public Entity Origin { get; set; }
    }
}