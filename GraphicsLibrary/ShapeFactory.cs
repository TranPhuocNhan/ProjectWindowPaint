using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLibrary
{
    public class ShapeFactory
    {
        public Dictionary<string, IShape> Prototypes = new Dictionary<string, IShape>();
        public IShape Create(string name)
        {
            IShape shape = Prototypes[name].CloneShape();
            return shape;
        }
    }
}
