using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class ArticleText
    {
        public string Name { get; }
        public string Genre { get; }
        public string TargetAudience { get; }
        public List<string> Content { get; }

        public ArticleText(string name, string genre, string targetAudience, List<string> content)
        {
            Name = name;
            Genre = genre;
            TargetAudience = targetAudience;
            Content = content;
        }

        public ArticleText(string name, string targetAudience, List<string> content) 
            : this(name, "", targetAudience, content) { }

        public ArticleText() : this("", "", "", new List<string>()) { }
    }
}
