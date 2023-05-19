using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Wizard_Unleashed
{
    public class LevelEditorDisplay : FrameworkElement
    {

        private Size size;

        public void SetupSizes(Size size)
        {
            this.size = size;
        }

        private int sizeOfMap = 16;
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            double rectWidth = size.Width / sizeOfMap;
            double rectHeight = size.Height / sizeOfMap;

            for (int i = 0; i < sizeOfMap; i++)
            {
                for (int j = 0; j < sizeOfMap; j++)
                {
                    drawingContext.DrawRectangle(
                                Brushes.White,
                                new Pen(Brushes.Black, 1),
                                new Rect(j * rectWidth, i * rectHeight, rectWidth , rectHeight )
                                );
                }
            }
        }
    }
}
