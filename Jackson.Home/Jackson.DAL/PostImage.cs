namespace Jackson.DAL
{
    public class PostImage
    {
        public int Id { get; set; }
      
        public byte[] Image { get; set; }
        public virtual BlogPost BlogPost { get; set; }
    }
}