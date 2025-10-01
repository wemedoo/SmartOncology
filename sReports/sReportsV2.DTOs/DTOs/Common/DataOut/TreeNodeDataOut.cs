using System;
using System.Collections.Generic;
using System.Drawing;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class TreeNodeDataOut
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string CircleColor { get; set; }
        public List<TreeNodeDataOut> Children { get; set; } = new();
        public TreeNodeDataOut()
        {
            CircleColor = GetRandomColor();
        }

        public void UpdateLabel(string label)
        {
            if (string.IsNullOrEmpty(Label))
            {
                Label = label;
            }
        }

        public bool IsLeaf()
        {
            return Children == null || Children.Count == 0;
        }

        private string GetRandomColor()
        {
            Random rnd = new Random();
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            return $"#{randomColor.R:X2}{randomColor.G:X2}{randomColor.B:X2}";
        }
    }
}
