using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public class AppSettings : ConfigurationSection
    {
        [ConfigurationProperty("mystem")]
        public string MyStemPath => (string) this["mystem"];

        [ConfigurationProperty("tagsCount")]
        public int TagsCount => (int) this["tagsCount"];

        [ConfigurationProperty("font")]
        public FontElement Font => (FontElement)this["font"];

        [ConfigurationProperty("color")]
        public ColorElement Color => (ColorElement)this["color"];
    }
    
    public class FontElement : ConfigurationElement
    {
        [ConfigurationProperty("fontFamily")]
        public String Family => (String)this["fontFamily"];

        [ConfigurationProperty("size", DefaultValue = "80", IsRequired = false)]
        public int Size => (int)this["size"];
    }
    
    public class ColorElement : ConfigurationElement
    {
        [ConfigurationProperty("background", IsRequired = true)]
        public String Background => (String)this["background"];

        [ConfigurationProperty("foreground", IsRequired = true)]
        public String Foreground => (String)this["foreground"];
    }

}
