using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

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
            //new Color(0.02352941f, 0.02352941f, 0.02352941f)//black
        };

        public static string[] colorNames = new string[]
        {
            "White",
            "Orange",
            "Magenta",
            "Light Blue",
            "Yellow",
            "Lime",
            "Pink",
            "Gray",
            "Light Gray",
            "Cyan",
            "Purple",
            "Blue",
            "Brown",
            "Green",
            "Red",
            "Black"
        };

        public static string[] smallNames = new string[]
        {
            "Kob",
            "Sunstreak",
            "Snooper",
            "Dasher",
            "Brinely",
            "Spotty"
        };

        public static string[] bigNames = new string[]
        {
            "Flopper",
            "Stripey",
            "Glitter",
            "Blockfish",
            "Betty",
            "Clayfish"
        };

        public static Material[] smallBaseMaterials = new Material[15];
        public static Material[] bigBaseMaterials = new Material[15];

        public static Material[,] smallDetailMaterials = new Material[6,15];
        public static Material[,] bigDetailMaterials = new Material[6,15];

        public Renderer[] renderers;

        public void Awake()
        {
            bool size = Random.Range(0, 2) == 1;
            int baseIndex = Random.Range(0, colors.Length);
            string baseColorName = colorNames[baseIndex];

            int detailIndex = baseIndex;
            while(detailIndex == baseIndex)
            {
                detailIndex = Random.Range(0, colors.Length);
            }

            int pattern = Random.Range(0, 6);

            gameObject.name += baseColorName + colorNames[detailIndex] + (size ? bigNames[pattern] : smallNames[pattern]);

            //string patternName;
            Material baseMat = size ? bigBaseMaterials[baseIndex] : smallBaseMaterials[baseIndex];
            Material detailMat = size ? bigDetailMaterials[pattern, detailIndex] : smallDetailMaterials[pattern, detailIndex];

            BoxCollider collider = GetComponent<BoxCollider>();
            collider.center = size ? Vector3.zero : new Vector3(0f, 0.05f, 0f);
            collider.size = size ? new Vector3(0.2f, 0.6f, 0.6f) : new Vector3(0.2f, 0.3f, 0.6f);

            foreach(Renderer renderer in renderers)
            {
                string rendName = renderer.name.ToLower();
                if(rendName.Contains(size ? "small" : "big"))
                {
                    Destroy(renderer.gameObject);
                    continue;
                }

                if(rendName.Contains("base"))
                {
                    renderer.material = baseMat;
                }
                if(rendName.Contains("detail"))
                {
                    renderer.material = detailMat;
                }
            }
        }
    }
}
