using System;
using System.Collections.Generic;
using System.Text;

namespace const_parser
{
    class DescriptionProvider
    {
        private LinkedList<string> currentDescList;

        public DescriptionProvider()
        {
            currentDescList = new LinkedList<string>();
        }

        public void AddDesc(string desc)
        {
            this.currentDescList.AddLast(desc);
        }

        public LinkedList<string> GetDescList()
        {
            var list = currentDescList;
            currentDescList = new LinkedList<string>();

            return list;
        }
    }
}
