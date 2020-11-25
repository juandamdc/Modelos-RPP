using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace GUI_Modelos_01
{
    public partial class Form1 : Form
    {
        List<PointF> puntos = new List<PointF>();
        List<PointF> puntosF = new List<PointF>();
        List<Figura> figurasadibujar = new List<Figura>();
        Color color = Color.Aqua;
        double radio = 20;
        Dictionary<string, Figura> diccionariofiguras = new Dictionary<string, Figura>();
        List<string> listafigurasVarias = new List<string>();
        Modo modo = Modo.Poligono;
        PointF? hover = null;
        string elementolistBox = null;
        int? elementolistBox2 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            var posicion=pictureBox1.PointToClient(Cursor.Position);
            puntos.Add(posicion);
            pictureBox1.Refresh();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            if (modo == Modo.Poligono)
            {
                PintarPolígono(puntos,e, color, true);
            }
            else if(modo==Modo.Elipse)
            {
                PintarElipse(puntos, e, color, true);
            }
            else if(modo==Modo.Regular)
            {
                PintarRegular(puntosF, e, color);
            }
            else if(modo==Modo.Conjunto)
            {
                PintarVarias(figurasadibujar,e);
            }

            //Pen pen = new Pen(Color.Black);
            //if (hover != null)
            //    e.Graphics.DrawLine(pen, new PointF(0, 0), (PointF)hover);
        }

        private RectangleF GenerarRectangulo(PointF punto1, PointF punto2)
        {
            PointF puntoinicial=new PointF();
            PointF puntofinal=new PointF();
            if(punto1.X<=punto2.X)
            {
                puntoinicial.X = punto1.X;
                puntofinal.X = punto2.X;
            }
            else
            {
                puntoinicial.X = punto2.X;
                puntofinal.X = punto1.X;
            }

            if(punto1.Y<=punto2.Y)
            {
                puntoinicial.Y = punto1.Y;
                puntofinal.Y = punto2.Y;
            }
            else
            {
                puntoinicial.Y = punto2.Y;
                puntofinal.Y = punto1.Y;
            }
            RectangleF rect = new RectangleF(puntoinicial, new SizeF(puntofinal.X - puntoinicial.X, puntofinal.Y - puntoinicial.Y));
            return rect;
        }

        private void PintarElipse(List<PointF> points, PaintEventArgs e, Color color, bool PintarCursor = false)
        {

            Pen pen = new Pen(Color.Black);
            Pen penlight = new Pen(Color.Beige);
            if(points.Count>1)
            {

                var rect = GenerarRectangulo(points[points.Count - 2], points[points.Count - 1]);
                var pen4 = new Pen(color);
                e.Graphics.FillEllipse(pen4.Brush,rect);
                pen4.Dispose();
                e.Graphics.DrawRectangle(penlight, rect.X, rect.Y, rect.Width, rect.Height);
                e.Graphics.DrawEllipse(pen, rect);
            }

            if (PintarCursor && hover != null&&points.Count>0)
            {
                Pen pen2 = new Pen(Color.Blue);
                var nuevorect = GenerarRectangulo(points[points.Count - 1], (PointF)hover);
                e.Graphics.DrawRectangle(penlight, nuevorect.X, nuevorect.Y, nuevorect.Width, nuevorect.Height);
                e.Graphics.DrawEllipse(pen2, nuevorect);
                pen2.Dispose();
            }
            pen.Dispose();
            penlight.Dispose();
        }

        private void PintarPolígono(List<PointF> points,PaintEventArgs e, Color color, bool PintarCursor=false)
        {
            Pen pen = new Pen(Color.Black);
            for (int i = 1; i < points.Count; i++)
            {
                e.Graphics.DrawLine(pen, points[i - 1], points[i]);
            }
            if (points.Count > 1)
            {
                var pen4 = new Pen(color);
                e.Graphics.FillPolygon(pen4.Brush, points.ToArray());
                pen4.Dispose();
                Pen pen3 = pen;
                if (hover != null && PintarCursor)
                    pen3 = new Pen(Color.FromArgb(150, 150, 150));
                e.Graphics.DrawLine(pen3, points[0], points[points.Count - 1]);
                if (hover != null && PintarCursor)
                    pen3.Dispose();
            }
            if (points.Count > 0 && hover != null && PintarCursor)
            {
                var punto = (PointF)hover;
                Pen pen2 = new Pen(Color.Blue);
                e.Graphics.DrawLine(pen2, punto, points[0]);
                e.Graphics.DrawLine(pen2, punto, points[points.Count - 1]);
                e.Graphics.DrawEllipse(pen2, punto.X - 2, punto.Y - 2, 6, 6);
                pen2.Dispose();
            }
            foreach (var punto in points)
            {
                e.Graphics.DrawEllipse(pen, punto.X - 2, punto.Y - 2, 6, 6);
            }
            pen.Dispose();
        }

        public void PintarVarias(List<Figura> figuras, PaintEventArgs e)
        {
            foreach(var fig in figuras)
            {
                if(fig is Poligono)
                {
                    var poli = (fig as Poligono);
                    PintarPolígono(poli.puntos, e, poli.color);
                }
                else if (fig is Elipse)
                {
                    var poli = (fig as Elipse);
                    PintarElipse(poli.puntos, e, poli.color);
                }
                else if (fig is Regular)
                {
                    var poli = (fig as Regular);
                    PintarRegular(poli.puntos, e, poli.color);
                }
            }
        }

        private void PintarRegular(List<PointF> points, PaintEventArgs e, Color color, bool PintarCursor = false)
        {
            if (points.Count < 3)
                return;
            var pen4 = new Pen(color);
            e.Graphics.FillPolygon(pen4.Brush, points.ToArray());
            pen4.Dispose();
            Pen pen = new Pen(Color.Black);
            for (int i = 1; i < points.Count; i++)
            {
                e.Graphics.DrawLine(pen, points[i - 1], points[i]);
            }
            e.Graphics.DrawLine(pen, points[points.Count - 1], points[0]);
            pen.Dispose();
        }


        private void PictureBox1_MouseEnter(object sender, EventArgs e)
        {
            hover = pictureBox1.PointToClient(Cursor.Position);
            pictureBox1.Refresh();
        }

        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            hover = null;
            pictureBox1.Refresh();
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            hover = pictureBox1.PointToClient(Cursor.Position);
            pictureBox1.Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (puntos.Count > 0)
            {
                puntos.RemoveAt(puntos.Count - 1);
                Refresh();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var nombre=textBox1.Text;
            if (modo == Modo.Poligono)
            {
                var copia = new List<PointF>();
                foreach(var p in puntos)
                    copia.Add(p);
                diccionariofiguras[nombre] = new Poligono(copia) { color=this.color, Name=nombre};
            }
            else if (modo == Modo.Elipse)
            {
                var copia = new List<PointF>();
                foreach (var p in puntos)
                    copia.Add(p);
                diccionariofiguras[nombre] = new Elipse(copia) { color = this.color, Name=nombre };
            }
            else if (modo == Modo.Regular)
            {
                var copia = new List<PointF>();
                foreach (var p in puntosF)
                    copia.Add(p);
                diccionariofiguras[nombre] = new Regular(){puntos=copia, color = this.color, radio=(double)numericUpDown2.Value, Name=nombre};
            }
            else if (modo == Modo.Conjunto)
            {
                diccionariofiguras[nombre] = new Conjunto(figurasadibujar) { Name=nombre};
            }
            if (!listBox1.Items.Contains(nombre))
                listBox1.Items.Add(nombre);
            listBox1.Refresh();
            Refresh();
        }
        public List<PointF> TransformaEscalaAPequeña (List<PointF> puntos)
        {
            List<PointF> resultado = new List<PointF>(puntos.Count);
            foreach(PointF p in puntos)
            {
                var nuevox = p.X * pictureBox2.Size.Width / pictureBox1.Size.Width;
                var nuevoy = p.Y * pictureBox2.Size.Height / pictureBox1.Size.Height;
                PointF nuevopunto = new PointF(nuevox, nuevoy);
                resultado.Add(nuevopunto);
            }
            return resultado;
        }

        public List<PointF> TransformaEscalaAPequeña(List<Point> puntos)
        {
            List<PointF> resultado = new List<PointF>(puntos.Count);
            foreach (Point p in puntos)
            {
                var nuevox = (float)(p.X * pictureBox2.Size.Width*1.0 / pictureBox1.Size.Width);
                var nuevoy = (float)(p.Y * pictureBox2.Size.Height*1.0 / pictureBox1.Size.Height);
                PointF nuevopunto = new PointF(nuevox, nuevoy);
                resultado.Add(nuevopunto);
            }
            return resultado;
        }

        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if(elementolistBox!=null&&diccionariofiguras.ContainsKey(elementolistBox))
            {
                e.Graphics.Clear(Color.White);
                var figura = diccionariofiguras[elementolistBox];
                if(figura.modo==Modo.Poligono)
                {
                    var poligono = figura as Poligono;
                    var puntospequenos = TransformaEscalaAPequeña(poligono.puntos);
                    PintarPolígono(puntospequenos, e, figura.color);
                }
                else if(figura.modo==Modo.Elipse)
                {
                    var elipse = figura as Elipse;
                    var puntospequenos = TransformaEscalaAPequeña(elipse.puntos);
                    PintarElipse(puntospequenos, e, figura.color);
                }
                else if(figura.modo==Modo.Regular)
                {
                    var elipse = figura as Regular;
                    var puntospequenos = TransformaEscalaAPequeña(elipse.puntos);
                    PintarRegular(puntospequenos, e, figura.color);
                }
            }
        }

        private void ListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            var nombre = listBox1.SelectedItem.ToString();
            elementolistBox = nombre;
            pictureBox2.Refresh();
        }

        private void ListBox1_Enter(object sender, EventArgs e)
        {
            //var nombre = listBox1.SelectedItem.ToString();
            //elementolistBox = nombre;
            //pictureBox2.Refresh();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if(elementolistBox!=null && diccionariofiguras.ContainsKey(elementolistBox))
            {
                var figura = diccionariofiguras[elementolistBox];
                this.color = figura.color;
                if (figura.modo == Modo.Poligono)
                {
                    groupBox1.Hide();
                    var poligono = figura as Poligono;
                    List<PointF> copia = new List<PointF>();
                    foreach (PointF p in poligono.puntos)
                        copia.Add(p);
                    puntos = copia;
                    modo = figura.modo;
                    radioButton4.Enabled = radioButton4.Checked = false;
                    radioButton1.Checked = true;
                    radioButton2.Checked = radioButton3.Checked = radioButton4.Checked=radioButton4.Enabled=false;
                }
                else if(figura.modo==Modo.Elipse)
                {
                    groupBox1.Hide();
                    var elipse = figura as Elipse;
                    List<PointF> copia = new List<PointF>();
                    foreach (PointF p in elipse.puntos)
                        copia.Add(p);
                    puntos = copia;
                    radioButton2.Checked = true;
                    radioButton4.Enabled = radioButton4.Checked =false;
                    radioButton1.Checked = radioButton3.Checked = radioButton4.Checked = radioButton4.Enabled = false;
                }
                else if(figura.modo==Modo.Regular)
                {
                    groupBox1.Show();
                    var regular = figura as Regular;
                    List<PointF> copia = new List<PointF>();
                    foreach (PointF p in regular.puntos)
                        copia.Add(p);
                    puntosF = copia;
                    radioButton4.Enabled = radioButton4.Checked = false;
                    radioButton3.Checked = true;
                    radioButton1.Checked = radioButton2.Checked = false;
                    numericUpDown1.Value = regular.puntos.Count;
                    numericUpDown2.Value = (decimal)regular.radio;
                    radioButton1.Checked = radioButton2.Checked = radioButton4.Checked = radioButton4.Enabled = false;
                    radio = regular.radio;
                    DibujarNuevoRegular();
                }
                else if(figura.modo==Modo.Conjunto)
                {
                    radioButton4.Enabled = radioButton4.Checked = true;
                    radioButton1.Checked = radioButton2.Checked = radioButton3.Checked = false;
                    figurasadibujar = ((Conjunto)figura).figuras;
                    modo = figura.modo;
                }
                pictureBox1.Refresh();
            }
        }

        private void ListBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //var nombre = listBox1.SelectedItem.ToString();
            //elementolistBox = nombre;
            //pictureBox2.Refresh();
        }

        private void ListBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //var nombre = listBox1.SelectedItem.ToString();
            //elementolistBox = nombre;
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton4.Checked = false;
                radioButton4.Enabled = false;
                groupBox1.Hide();
                modo = Modo.Poligono;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                puntos = new List<PointF>();
                pictureBox1.Refresh();
            }
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton4.Checked = false;
                radioButton4.Enabled = false;
                groupBox1.Hide();
                modo = Modo.Elipse;
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                puntos = new List<PointF>();
                pictureBox1.Refresh();
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            puntos = new List<PointF>();
            pictureBox1.Refresh();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            DibujarNuevoRegular();
        }

        public void DibujarNuevoRegular()
        {
            modo = Modo.Regular;
            var figura = new Regular((int)numericUpDown1.Value, (double)numericUpDown2.Value);
            puntosF = figura.puntos;
            pictureBox1.Refresh();
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                radioButton4.Checked = false;
                radioButton4.Enabled = false;
                groupBox1.Show();
                modo = Modo.Regular;
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                puntosF = new List<PointF>();
                DibujarNuevoRegular();
                pictureBox1.Refresh();
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            var result=colorDialog1.ShowDialog();
            color = colorDialog1.Color;
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            radio = (double)numericUpDown2.Value;
            DibujarNuevoRegular();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DibujarNuevoRegular();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            radioButton4.Enabled = true;
            radioButton4.Checked = true;
            radioButton1.Checked = radioButton2.Checked = radioButton3.Checked = false;
            modo = Modo.Conjunto;

            List<MyRectangle> rectangulos = new List<MyRectangle>();
            foreach(var elemento in listBox2.Items)
            {
                var figura = diccionariofiguras[elemento.ToString()];
                if(figura is FiguraDibujable)
                {
                    var dibujable = (FiguraDibujable)figura;
                    var rectangulo = dibujable.GenerarRectanguloAsociado();
                    rectangulos.Add(rectangulo);
                }
            }

            var sk = new Skyline(pictureBox1.Size.Width, pictureBox1.Height);
            var result = sk.Run(rectangulos);
            if(result.Item1)
            {
                var varios = new Conjunto(result.Item2);
                figurasadibujar = varios.figuras;
                pictureBox1.Refresh();
            }
            return;
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if(elementolistBox!=null)
            {
                listBox2.Items.Add(elementolistBox);
                listBox2.Refresh();
            }
        }

        private void ListBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null)
            {
                elementolistBox2 = null;
                return;
            }
            elementolistBox2 = listBox1.SelectedIndex;
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            if (elementolistBox2 == null)
                return;
            listBox2.Items.RemoveAt((int)elementolistBox2);
            listBox2.Refresh();
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            List<Figura> ASalvar = new List<Figura>();
            foreach(var item in listBox1.Items)
            {
                var figura = diccionariofiguras[item.ToString()];
                figura.Name = item.ToString();
                ASalvar.Add(figura);
            }
            Type[] tipos = { typeof(Figura), typeof(Color),typeof(Poligono), typeof(Elipse), typeof(Regular), typeof(Conjunto) };

            XmlSerializer serializer = new XmlSerializer(typeof(List<Figura>), tipos);
            StreamWriter archivo = new StreamWriter(saveFileDialog1.FileName);
            serializer.Serialize(archivo, ASalvar);
            archivo.Flush();
            archivo.Close();
            return;
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            Type[] tipos = { typeof(Figura), typeof(Color), typeof(Poligono), typeof(Elipse), typeof(Regular), typeof(Conjunto) };

            XmlSerializer serializer = new XmlSerializer(typeof(List<Figura>), tipos);
            StreamReader archivo = new StreamReader(openFileDialog1.FileName);
            var result=(List<Figura>)serializer.Deserialize(archivo);
            HashSet<string> listboxElements = new HashSet<string>();
            foreach (var item in listBox1.Items)
            {
                listboxElements.Add(item.ToString());
            }

            foreach (var fig in result)
            {
                diccionariofiguras[fig.Name] = fig;
                if(!listboxElements.Contains(fig.Name))
                {
                    listBox1.Items.Add(fig.Name);
                }
            }
            archivo.Close();
            listBox1.Refresh();
            return;
        }

        private void Button5_Click_1(object sender, EventArgs e)
        {
            if (elementolistBox == null)
                return;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.Items[i].ToString() == elementolistBox)
                {
                    listBox1.Items.RemoveAt(i);
                    listBox1.Refresh();
                    return;
                }
            }
        }
    }
    public enum Modo
    {
        Poligono,
        Elipse,
        Regular,
        Conjunto
    }
    public class Figura
    {
        [XmlIgnore]
        public Color color;
        public string Name = "";
        public Modo modo;
        [XmlElement("color")]
        public int colorAsArgb
        {
            get { return color.ToArgb(); }
            set { color = Color.FromArgb(value); }
        }
    }

    public abstract class FiguraDibujable:Figura
    {
        public List<PointF> puntos = new List<PointF>();
        public abstract MyRectangle GenerarRectanguloAsociado();
    }
    public class Poligono : FiguraDibujable
    {
        public Poligono()
        {
            modo = Modo.Poligono;
        }
        public Poligono(List<PointF> puntos)
        {
            modo = Modo.Poligono;
            this.puntos = puntos;
        }

        public override MyRectangle GenerarRectanguloAsociado()
        {
            var Xs = from punto in puntos
                     select punto.X;
            var Ys = from punto in puntos
                     select punto.Y;

            var Xminima = Xs.Min();
            var Yminima = Ys.Min();

            var puntosfinales = from punto in puntos
                                select new PointF(punto.X - Xminima, punto.Y - Yminima);

            var XsNuevos = from punto in puntosfinales
                     select punto.X;
            var YsNuevos = from punto in puntosfinales
                     select punto.Y;
            var rectwidth = XsNuevos.Max();
            var rectheight = YsNuevos.Max();

            var nuevoPoligono = new Poligono(puntosfinales.ToList());
            nuevoPoligono.color = this.color;

            var rectangulo = new MyRectangle(rectwidth, rectheight, nuevoPoligono);
            return rectangulo;
        }
    }
    public class Elipse : FiguraDibujable
    {
        public Elipse()
        {
            modo = Modo.Elipse;
        }
        public Elipse(List<PointF> puntos)
        {
            modo = Modo.Elipse;
            this.puntos = puntos;
        }
        public override MyRectangle GenerarRectanguloAsociado()
        {
            var puntos = new List<PointF>();
            puntos.Add(this.puntos[this.puntos.Count - 2]);
            puntos.Add(this.puntos[this.puntos.Count - 1]);

            var Xs = from punto in puntos
                     select punto.X;
            var Ys = from punto in puntos
                     select punto.Y;

            var Xminima = Xs.Min();
            var Yminima = Ys.Min();

            var puntosfinales = from punto in puntos
                                select new PointF(punto.X - Xminima, punto.Y - Yminima);

            var XsNuevos = from punto in puntosfinales
                           select punto.X;
            var YsNuevos = from punto in puntosfinales
                           select punto.Y;
            var rectwidth = XsNuevos.Max();
            var rectheight = YsNuevos.Max();

            var nuevoPoligono = new Elipse(puntosfinales.ToList());
            nuevoPoligono.color = this.color;

            var rectangulo = new MyRectangle(rectwidth, rectheight, nuevoPoligono);
            return rectangulo;
        }
    }
    public class Regular :FiguraDibujable
    {
        public double radio;
        public Regular()
        {
            modo = Modo.Regular;
            radio = 20;
        }
        public Regular(int cantidadLados,double radio=20)
        {
            modo = Modo.Regular;
            this.radio = radio;
            for(int i=0; i<cantidadLados; i++)
            {
                PointF punto = new PointF();
                var radianes = 2 * i * Math.PI / cantidadLados;
                punto.X = (float)(Math.Cos(radianes)*radio);
                punto.Y = (float)(Math.Sin(radianes)*radio);
                puntos.Add(punto);
            }
            Ajustar();
        }
        public void Ajustar()
        {
            var menorX = puntos.Min(elemento => elemento.X);
            var menorY = puntos.Min(elemento => elemento.Y);
            for(int i=0; i<puntos.Count; i++)
            {
                var nuevopunto = new PointF(puntos[i].X - menorX, puntos[i].Y - menorY);
                puntos[i] = nuevopunto;
            }
        }
        public override MyRectangle GenerarRectanguloAsociado()
        {
            var Xs = from punto in puntos
                     select punto.X;
            var Ys = from punto in puntos
                     select punto.Y;

            var Xminima = Xs.Min();
            var Yminima = Ys.Min();

            var puntosfinales = from punto in puntos
                                select new PointF(punto.X - Xminima, punto.Y - Yminima);

            var XsNuevos = from punto in puntosfinales
                           select punto.X;
            var YsNuevos = from punto in puntosfinales
                           select punto.Y;
            var rectwidth = XsNuevos.Max();
            var rectheight = YsNuevos.Max();

            var nuevoPoligono = new Poligono(puntosfinales.ToList());
            nuevoPoligono.color = this.color;

            var rectangulo = new MyRectangle(rectwidth, rectheight, nuevoPoligono);
            return rectangulo;
        }
    }
    public class Conjunto :Figura
    {
        public List<Figura> figuras = new List<Figura>();
        public Conjunto()
        {
            modo = Modo.Conjunto;
        }

        public Conjunto(IEnumerable<Figura> figuras)
        {
            modo = Modo.Conjunto;
            this.figuras = figuras.ToList();
        }
        public Conjunto(IEnumerable<MyRectangle> rectangulos)
        {
            modo = Modo.Conjunto;
            foreach(var rect in rectangulos)
            {
                var figura = (FiguraDibujable)rect.figura;
                var nuevospuntos = from punto in figura.puntos
                                   select new PointF((float)(punto.X + rect.InferiorLeftPoint.Item1), (float)(punto.Y + rect.InferiorLeftPoint.Item2));
                if(rect.Rotate)
                {
                    nuevospuntos = from punto in nuevospuntos
                                   select new PointF(punto.Y, punto.X);
                }
                figura.puntos = nuevospuntos.ToList();
                figuras.Add(figura);
            }
        }
    }
}
