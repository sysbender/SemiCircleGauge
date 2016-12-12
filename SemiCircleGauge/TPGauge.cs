using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace Tp1_819_mengjianjun

{
    // default event is fired when value change
    [ToolboxBitmapAttribute(typeof(TPGauge), "TPGauge.bmp"),
    DefaultEvent("ValueChanged"),
    Description("show value on an analog gauge.")]

    public partial class TPGauge : Control
    {
        #region variables
        public enum FrameTypeEnum
        {
            circle = 0,
            semi_circle =1,
            rectangle=2
        }
        private Boolean drawGaugeBackground = true;

        private Bitmap gaugeBitmap;
        //value 
        private Single  _value = 50;
        private Single minValue = 0;
        private Single maxValue = 200;

        private Color gaugeColor = ColorTranslator.FromHtml("#d4def3");

        //unit for size  ( unit = width/20)
        private int unit=10;
        
        // position
        private Point center = new Point(130,130);

        //  frame
        //private FrameTypeEnum frameType = FrameTypeEnum.circle;
        private Color frameColor = ColorTranslator.FromHtml("#81868a");
        private Int32 frameWidth = 20;   // line 
        //private Int32 framePadding = 20;    // between frame and arc
        private Int32 frameRadius = 120;

        // base arc
        private Color arcColor = ColorTranslator.FromHtml("#c1c9f2");
        private Int32 arcRadius = 80;
        private Int32 arcStart = 135;
        private Int32 arcSweep = 270;
        private Int32 arcWidth = 10;


        //scale line inter
        private Color interColor = ColorTranslator.FromHtml("#9aa4b3");
        private Int32 interInnerRadius = 73;
        private Int32 interOuterRadius = 80;
        private Int32 interWidth = 2;


        // scale line minor
        private Int32 minorNumOf = 5;

        private Color minorColor =ColorTranslator.FromHtml("#9aa4b3");
        private Int32 minorInnerRadius = 75;
        private Int32 minorOuterRadius = 80;
        private Int32 minorWidth = 1;

        // scale line major
        private Single majorStepValue = 50.0f;


        private Color majorColor = ColorTranslator.FromHtml("#9aa4b3");
        private Int32 majorInnerRadius = 70;
        private Int32 majorOuterRadius = 80;
        private Int32 majorWidth = 2;

        // scale number
        private Int32 numberRadius = 95;

        //needle
        private Int32 needleType = 0;
        private Int32 needleRadius = 80;
        private Color needleColor1 = ColorTranslator.FromHtml("#f85227");
        private Color needleColor2 = Color.DimGray;
        private Int32 needleWidth = 5;



        #endregion //variables
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                CP.ExStyle |= 0x20;
                return CP;
            }
        }

        public TPGauge()
        {
            InitializeComponent();
            Width = 260;
            Height = 224;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = ColorTranslator.FromHtml("#d4def3");
            //gaugeColor = BackColor;
            ForeColor =  ColorTranslator.FromHtml("#c1c9f2");
            //arcColor = ForeColor;

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Opaque, true);
            //this.BackColor = Color.Transparent;
        }

        #region define properties
        [Browsable(true),
            Category("TPGauge"),
            RefreshProperties(RefreshProperties.All),
            Description(" value ")]

        public Single Value
        {
            get { return _value; }
            set
            {
                if(value != _value)
                {
                    if (value < minValue) _value = minValue;
                    else if (value > maxValue) _value = maxValue;
                    else _value = value;

                    if (this.DesignMode) { drawGaugeBackground = true; }

                    Refresh();

                }
            }
        }

        [Browsable(true),
    Category("TPGauge"),
    RefreshProperties(RefreshProperties.All),
    Description(" center position ")]
        public Point Center
        {
            get { return center; }
            set
            {
                if(center != value)
                {
                    center = value;
                    drawGaugeBackground = true;
                    Refresh();
            }
            }
        }

        public Single MinValue
        {
            get { return minValue; }
            set
            {
                if (value !=minValue && value <maxValue ) {
                    minValue = value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        public Single MaxValue
        {
            get { return maxValue; }
            set
            {
                if (value != maxValue && value > minValue)
                {
                    maxValue = value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
    Category("TPGauge"),
    RefreshProperties(RefreshProperties.All),
    Description(" Number of major scale line ")]
        private int majorNumOf=5;

        public int MajorNumOf
        {
            get { return majorNumOf; }
            set {
                majorNumOf = value;
                majorStepValue = maxValue / MajorNumOf;
                Refresh();
            }
        }


        #endregion //properties


        #region methods
        protected override void OnPaintBackground(PaintEventArgs pe)

        {
            
          
        }
        /**
         * calculater size of shapes 
         */
        protected void CalculateSize()
        {
            unit = (int) (ClientRectangle.Width/20);
            center = new Point(10*unit, 10*unit);
            // frame
            frameWidth = unit;
            frameRadius= unit * 9;

            // arc
            arcRadius = unit * 6;
            arcWidth = (int)(unit / 2);

            //
            minorInnerRadius = arcRadius - arcWidth ;
            minorOuterRadius = arcRadius + arcWidth;


            interInnerRadius = minorInnerRadius  - 2;
            interOuterRadius = minorOuterRadius + 2;

            majorInnerRadius = minorInnerRadius  - 4;
            majorOuterRadius = minorOuterRadius + 4;

            numberRadius = majorOuterRadius + 10;

            needleRadius = minorInnerRadius - 5;

        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            CalculateSize();
            // base.OnPaint(e);
            //Graphics g = pe.Graphics;
            // g.FillRectangle(new SolidBrush(Color.Red), ClientRectangle);
            // g.DrawRectangle(new Pen(Color.Blue, 1.0f), ClientRectangle);

            //base.OnPaintBackground(pevent);
            if(drawGaugeBackground)
            {
                //drawGaugeBackground = false;
                // instaniate a bitmap 
                gaugeBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, pe.Graphics);
                gaugeBitmap.MakeTransparent();
                //create graphics from image
                Graphics g = Graphics.FromImage(gaugeBitmap);
                // set bitmap background
                g.Clear(this.Parent.BackColor);

                // draw frame 
                DrawGaugeFrame(g);

                // draw arc
                g.SetClip(ClientRectangle);
                if (arcRadius > 0)
                {
                    DrawGaugeArc(g);
                }


                // draw Scale lines and number
                DrawGaugeScaleLines(g);

                // draw needle
                DrawGaugeNeedle(g);


                // Graphics g = pe.Graphics;
                // fill the area of the control with background color
                //g.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

                // rending bitmap to a control - refer http://www.techotopia.com/index.php/Using_Bitmaps_for_Persistent_Graphics_in_C_Sharp#Rendering_a_Bitmap_Image_on_a_Control
                pe.Graphics.DrawImageUnscaled(gaugeBitmap, 0, 0);
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            }
        }

        private void DrawGaugeFrame (Graphics g)
        {
            // graph path
            GraphicsPath gpFrame = new GraphicsPath();
            gpFrame.StartFigure();
            // add arc to graph path
            gpFrame.AddArc(new Rectangle(center.X - frameRadius, center.Y - frameRadius, 2 * frameRadius, 2 * frameRadius), arcStart, arcSweep);
            // closeFigure will add a line automatically
            gpFrame.CloseFigure();

            // draw path
            g.DrawPath(new Pen(frameColor, frameWidth),gpFrame);
            g.FillPath(new SolidBrush(BackColor), gpFrame);

            //g.DrawRectangle(new Pen(Color.Blue, 10), ClientRectangle);
            // g.DrawArc(new Pen(arcColor, frameWidth),    new Rectangle(center.X - frameRadius, center.Y - frameRadius, 2 * frameRadius, 2 * frameRadius),    arcStart,    arcSweep);

        }

        private void DrawGaugeArc(Graphics g)
        {
            g.DrawArc(new Pen(ForeColor, arcWidth),
                new Rectangle(center.X - arcRadius, center.Y - arcRadius, 2 * arcRadius, 2 * arcRadius),
                arcStart,
                arcSweep);
        }
        private void DrawGaugeNumber(Graphics g, String text, Int32 angle)
        {

            Int32 rectHeight=15;

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            //e.Graphics.DrawString(text1, font1, Brushes.Blue, rect1, stringFormat);


            Font f = new Font(DefaultFont, FontStyle.Regular);
            Brush b = new SolidBrush(Color.Black);
            Double radian = angle * Math.PI / 180;

            Int32 x = center.X + (Int32)( numberRadius * Math.Cos(radian));
            Int32 y = center.Y + (Int32)( numberRadius * Math.Sin(radian));
            Rectangle rect = new Rectangle(x-2*rectHeight, y-rectHeight/2, 4* rectHeight, rectHeight);
            g.DrawString(text,f,b, rect, stringFormat);
            
        }

        private void DrawGaugeScaleLine(Graphics g, Color color, Int32 width, Int32 radius1, Int32 radius2, Int32 angle)
        {
            Double radian = angle * Math.PI / 180;
            Point p1 = new Point();
            Point p2 = new Point();
            p1.X = center.X + (Int32)(radius1 * Math.Cos(radian));
            p1.Y = center.Y + (Int32)(radius1 * Math.Sin(radian));

            p2.X = center.X + (Int32)(radius2 * Math.Cos(radian));
            p2.Y = center.Y + (Int32)(radius2 * Math.Sin(radian));

            g.DrawLine(new Pen(color, width), p1, p2);
        }

        private void DrawGaugeScaleLines(Graphics g)
        {

            Single interStepValue = majorStepValue / 2;
            Single minorStepValue = interStepValue / minorNumOf;

            Int32 angleCounter; ;
            Single valueCounter = minValue;
            do
            {
               
                // draw major
                angleCounter = arcStart + (Int32)(arcSweep * (valueCounter - minValue) / (maxValue - minValue));
                angleCounter = angleCounter % 360;
                DrawGaugeScaleLine(g, majorColor, majorWidth, majorInnerRadius, majorOuterRadius, angleCounter);
                // draw number
                DrawGaugeNumber(g, valueCounter.ToString(), angleCounter);
                // draw minor
                Single tempValue=valueCounter;
                for (int i =1; i<minorNumOf; i++)
                {
                    tempValue += minorStepValue;
                    angleCounter = arcStart + (Int32)(arcSweep * (tempValue - minValue) / (maxValue - minValue));
                    angleCounter = angleCounter % 360;
                    DrawGaugeScaleLine(g, minorColor, minorWidth, minorInnerRadius, minorOuterRadius, angleCounter);

                }

                // draw inter
                tempValue += minorStepValue;
                angleCounter = arcStart + (Int32)(arcSweep * (tempValue - minValue) / (maxValue - minValue));
                angleCounter = angleCounter % 360;
                DrawGaugeScaleLine(g, interColor, interWidth, interInnerRadius, interOuterRadius, angleCounter);



                // draw minor
                for (int i = 1; i < minorNumOf; i++)
                {
                    tempValue += minorStepValue;
                    angleCounter = arcStart + (Int32)(arcSweep * (tempValue - minValue) / (maxValue - minValue));
                    angleCounter = angleCounter % 360;
                    DrawGaugeScaleLine(g, minorColor, minorWidth, minorInnerRadius, minorOuterRadius, angleCounter);

                }
                // step to next major
                valueCounter += majorStepValue;

                
            } while (valueCounter < maxValue);


            // draw major - last
            angleCounter = arcStart + (Int32)(arcSweep * (valueCounter - minValue) / (maxValue - minValue));
            angleCounter = angleCounter % 360;
            DrawGaugeScaleLine(g, majorColor, majorWidth, majorInnerRadius, majorOuterRadius, angleCounter);
            // draw number
            DrawGaugeNumber(g, valueCounter.ToString(), angleCounter);



        }


        private void DrawGaugeNeedle(Graphics g)
        {


            // draw needle
            Int32 angle = arcStart + (Int32)(arcSweep * (_value - minValue) / (maxValue - minValue));
            angle  = angle  % 360;
            DrawGaugeScaleLine(g, Color.Orange, needleWidth, 0, needleRadius, angle );
            // draw cap
            Int32 capSize = 10;
            Rectangle cap = new Rectangle(center.X - capSize, center.Y - capSize, capSize * 2, capSize * 2);

            g.DrawEllipse(new Pen(Color.DarkGray), cap);
            g.FillEllipse(new SolidBrush(Color.Gray), cap);
        }
        #endregion // methods

    }
}
