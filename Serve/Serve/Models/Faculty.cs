//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Serve.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Faculty
    {
        public Faculty()
        {
            this.Unit = new HashSet<Unit>();
        }
    
        public int facultyId { get; set; }
        public string facultyName { get; set; }
    
        public virtual ICollection<Unit> Unit { get; set; }
    }
}
