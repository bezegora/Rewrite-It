using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rewrite_It
{
    class ArticleText
    {
        public Label Name { get; }
        public Label Genre { get; }
        public Label TargetAudience { get; }
        public Label[] Content { get; }

        public ArticleText(Label name, Label genre, Label targetAudience, Label[] content)
        {
            Name = name;
            Genre = genre;
            TargetAudience = targetAudience;
            Content = content;
        }

        public ArticleText(Label name, Label targetAudience, Label[] content) 
            : this(name, new Label(), targetAudience, content) { }

        public ArticleText() : this(new Label(), new Label(), new Label(), new Label[0]) { }
    }
}
