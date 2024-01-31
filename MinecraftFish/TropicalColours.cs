using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MinecraftFish
{
    public class TropicalColours : MonoBehaviour
    {
        public static Color[] colors = new Color[]
        {
            new Color(0.8078432f,0.8078432f,0.8078432f),//white
            new Color(0.8392158f, 0.3529412f, 0.06666665f),//orange
            new Color(0.5843138f, 0.1490196f, 0.5411765f),//magenta
            new Color(0.145098f, 0.4823529f, 0.7333333f),//light blue
            new Color(0.9254903f, 0.6705883f, 0.1098039f),//yellow
            new Color(0.3254901f, 0.6235294f, 0.09803919f),//lime
            new Color(0.7921569f, 0.3607843f, 0.517647f),//pink
            new Color(0.1647058f, 0.1647058f, 0.1647058f),//gray
            new Color(0.4431373f, 0.4431373f, 0.4431373f),//light gray
            new Color(0.07843135f, 0.4f, 0.4745098f),//cyan
            new Color(0.3411765f, 0.09803919f, 0.545098f),//purple
            new Color(0.1372549f, 0.1333333f, 0.490196f),//blue
            new Color(0.3254901f, 0.1882353f, 0.0941176f),//brown
            new Color(0.2352941f, 0.3019608f, 0.1137255f),//green
            new Color(0.4784313f, 0.1019608f, 0.0941176f),//red
            new Color(0.02352941f, 0.02352941f, 0.02352941f)//black
        };

        public static List<Material> materials = new List<Material>();

        public static Material eye;

        public void Awake()
        {
            Material main = materials[Random.Range(0, materials.Count)];
            Material detail = materials[Random.Range(0, materials.Count)];

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                string name = renderer.name.ToLower();

                if (name.Contains("main"))
                {
                    renderer.material = main;
                }
                else if (name.Contains("detail"))
                {
                    renderer.material = detail;
                }
                else if (name.Contains("eye"))
                {
                    renderer.material = eye;
                }
            }
        }
    }
}
