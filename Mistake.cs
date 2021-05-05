using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rewrite_It
{
    public class Mistake
    {
        public MistakeType 

        /// <summary>
        /// Содержит все виды ошибок, которые могут встретиться в тексте
        /// </summary>
        public enum MistakeType
        {
            None,
            NoNumbers,
            IncorrectDefinitionTargetAudience
        }
    }
}
