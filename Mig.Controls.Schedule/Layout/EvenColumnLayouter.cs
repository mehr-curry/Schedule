/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 20:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
    /// <summary>
    /// Description of EqualColumnLayouter.
    /// </summary>
    public class EvenColumnLayouter : DependencyObject, IColumnLayouter
    {
        public EvenColumnLayouter()
        {

        }

        public double ColumnMinWidth
        {
            get { return (double)GetValue(ColumnMinWidthProperty); }
            set { SetValue(ColumnMinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnMinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnMinWidthProperty =
            DependencyProperty.Register("ColumnMinWidth", typeof(double), typeof(EvenColumnLayouter), new UIPropertyMetadata(25D));

        
        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(EvenColumnLayouter), new UIPropertyMetadata(100D));



        public double ColumnMaxWidth
        {
            get { return (double)GetValue(ColumnMaxWidthProperty); }
            set { SetValue(ColumnMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnMaxWidthProperty =
            DependencyProperty.Register("ColumnMaxWidth", typeof(double), typeof(EvenColumnLayouter), new UIPropertyMetadata(200D));

        private double _defaultWidth;


        public Schedule Owner { get; set; }

        public void SetAll(double width)
        {
            foreach (ScheduleColumn col in Owner.Columns)
                col.SetCurrentValue(ScheduleColumn.WidthProperty, width);

            Owner.InvalidateMeasure();
        }

        public void Calculate(ScheduleColumn column, double change)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (!Owner.Columns.Contains(column))
                throw new ArgumentOutOfRangeException("column");

            var width = ColumnWidth + change / (Owner.Columns.IndexOf(column) + 1);

            if (width > ColumnMaxWidth)
                width = ColumnMaxWidth;
            else if (width < ColumnMinWidth)
                width = ColumnMinWidth;

            ColumnWidth = width;
            SetAll(width);

        }

        public IEnumerable<ScheduleColumn> GetVisibleColumns(Rect viewport)
        {
            return from col in Owner.Columns let yOffset = TranslateFromSource(col.Value) where yOffset < viewport.Right && yOffset + ColumnWidth > viewport.Left select col;
        }

        public double GetDesiredWidth()
        {
            if (Owner == null)
                throw new InvalidOperationException("Kein Besitzer");
            //if(!Owner.Columns.Any())
            //    throw new InvalidOperationException("Keine Spalten");

            return Owner.Columns.Any() ? Owner.Columns.Count * ColumnWidth : 0D;
        }

        public double TranslateFromSource(object value)
        {
            return SnappingBehavior.TranslateFromSource(value);
            //            double offset = 0;
            //
            //            foreach (ScheduleColumn col in Owner.Columns)
            //            {
            //                if (!Equals(col.Value, value))
            //                    offset += col.Width;
            //                else
            //                    break;
            //            }
            //
            //            return offset;
        }

        public object TranslateToSource(double horizontalValue)
        {
            return SnappingBehavior.TranslateToSource(horizontalValue);

            //            if (Owner.Columns.Any())
            //            {

            //                ScheduleColumn col = null;
            //                double offset = 0D;
            //
            //                foreach (var c in Owner.Columns)
            //                {
            //                    offset += c.Width;
            //
            //                    if (offset >= horizontalValue)
            //                        return c.Value;
            //                }

            //var colIdx = (int)Math.Floor(horizontalValue / Owner.Columns[0].Width);

            //if(colIdx < 0)
            //    colIdx = 0;
            //else
            //    if(colIdx >= Owner.Columns.Count)
            //        colIdx = Owner.Columns.Count-1;

            //return Owner.Columns[colIdx].Value;
            // }

            //            return null;
        }

        public ISnappingBehavior SnappingBehavior { get; set; }

        public double DefaultWidth
        {
            get { return ColumnWidth; }
        }
    }


}
