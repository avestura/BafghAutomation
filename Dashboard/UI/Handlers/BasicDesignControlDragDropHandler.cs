using Dashboard.UI.Adorners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dashboard.UI.Handlers
{
    public class BasicDesignControlDragDropHandler
    {
        public bool IsDragging { get; private set; }

        public bool IsDown { get; private set; }

        public double ZoomLevel { get; set; } = 1;

        public UIElement DraggingElement { get; private set; }

        private Point startPoint = new Point();
        private DraggingElementInfo draggingInfo = new DraggingElementInfo();
        private DesignAdorner designAdorner;

        public bool ConstraintToBounds { get; set; } = false;

        public Point ConstraintArea { get; set; } = new Point(0, 0);

        public Canvas DragArea { get; }

        public BasicDesignControlDragDropHandler(Canvas dragArea)
        {
            DragArea = dragArea;

            dragArea.PreviewMouseLeftButtonDown += DragArea_PreviewMouseLeftButtonDown;
            dragArea.PreviewMouseMove += DragArea_PreviewMouseMove;
            dragArea.PreviewMouseLeftButtonUp += DragArea_PreviewMouseLeftButtonUp;
        }

        public void RemoveEventHandlers()
        {
            DragArea.PreviewMouseLeftButtonDown -= DragArea_PreviewMouseLeftButtonDown;
            DragArea.PreviewMouseMove -= DragArea_PreviewMouseMove;
            DragArea.PreviewMouseLeftButtonUp -= DragArea_PreviewMouseLeftButtonUp;
        }

        private void DragArea_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Don't move when thumb clicked
            var thumb = (e.OriginalSource is Border b && b.Parent is Grid g && g.TemplatedParent is Thumb);
            if (e.Source != DragArea && !thumb)
            {
                IsDown = true;
                startPoint = e.GetPosition(DragArea);
                DraggingElement = e.Source as UIElement;
                DragArea.CaptureMouse();
                e.Handled = true;
            }
        }

        private void DragArea_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsDown)
            {
                if ((!IsDragging) &&
                    ((Math.Abs(e.GetPosition(DragArea).X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(DragArea).Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (IsDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragArea_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsDown)
            {
                DragFinished();
                e.Handled = true;
            }
        }

        private void DragStarted()
        {
            IsDragging = true;
            draggingInfo.Left = Canvas.GetLeft(DraggingElement);
            draggingInfo.Top = Canvas.GetTop(DraggingElement);

            designAdorner = new DesignAdorner(DraggingElement);
            var layer = AdornerLayer.GetAdornerLayer(DraggingElement);
            layer.Add(designAdorner);
        }

        private void DragMoved()
        {
            var currentPosition = Mouse.GetPosition(DragArea);

            designAdorner.LeftOffset = (currentPosition.X - startPoint.X) * ZoomLevel;
            designAdorner.TopOffset = (currentPosition.Y  - startPoint.Y) * ZoomLevel;
        }

        private void DragFinished()
        {
            Mouse.Capture(null);
            if (IsDragging)
            {
                AdornerLayer.GetAdornerLayer(designAdorner.AdornedElement).Remove(designAdorner);

                var newTop = draggingInfo.Top + designAdorner.TopOffset / ZoomLevel;
                var newLeft = draggingInfo.Left + designAdorner.LeftOffset / ZoomLevel;

                Canvas.SetTop(DraggingElement, newTop);
                Canvas.SetLeft(DraggingElement, newLeft);

                designAdorner = null;
            }

            if (ConstraintToBounds && !ElementWithin(DraggingElement, DragArea))
            {
                DragArea.Children.Remove(DraggingElement);
                DraggingElement = null;
            }

            IsDragging = false;
            IsDown = false;
        }

        private bool ElementWithin(UIElement child, Canvas parent)
        {
            var relativeLocation = new Point(Canvas.GetLeft(child), Canvas.GetTop(child));
            return relativeLocation.X >= 0 && relativeLocation.Y >= 0
                   && relativeLocation.X <= ConstraintArea.X
                   && relativeLocation.Y <= ConstraintArea.Y;
        }
    }

    public struct DraggingElementInfo
    {
        public double Top { get; set; }

        public double Left { get; set; }
    }
}
