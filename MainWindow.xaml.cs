using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.Windows;

namespace Allegorical_Greyness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        string path, pathToHide, pathToDecode;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void go_btn_Click(object sender, RoutedEventArgs e)
        {
            if (path != null && path != "" && pathToHide != null && pathToHide != "")
            {
                int red, green, blue, newRed, newGreen, newBlue, averageColor;
                int height, width, heightToHide, widthToHide;
                int bitmeter = 0, temp;

                Color getColor = new Color();
                Bitmap bitmap = new Bitmap(path);               //'bitmap' is an original image
                Bitmap bitmapToHide = new Bitmap(pathToHide);   //'bitmapToHide' is an original version of image destined to hide

                height = bitmap.Height;
                width = bitmap.Width;
                heightToHide = bitmapToHide.Height;
                widthToHide = bitmapToHide.Width;


                if (heightToHide * widthToHide <= (height * 3 * width / 8) - 10) //The width and height of the hidden message is written on 30 bits = 10 pixels.
                {
                    //Conversion of 'image to hide' to greyscale
                    int[] intmessage = new int[heightToHide * widthToHide];     //'intmessage' is 'bitmapToHide' converted to greyscale in bool array
                    Color color = new Color();
                    bitmeter = 0;

                    for (int i = 0; i < heightToHide; i++)
                    {
                        for (int j = 0; j < widthToHide; j++)
                        {
                            getColor = bitmapToHide.GetPixel(j, i);
                            red = getColor.R;
                            green = getColor.G;
                            blue = getColor.B;
                            averageColor = (red + green + blue) / 3;

                            intmessage[bitmeter++] = averageColor;
                        }
                    }


                    //Copying 'intmessage' array into bool array named 'boolmessage'
                    bool[] boolmessage = new bool[heightToHide * widthToHide * 8];
                    int multiplier = 128, bytemeter = 0;
                    bitmeter = 0;

                    for (int i = 0; i < heightToHide; i++)
                    {
                        for (int j = 0; j < widthToHide; j++)
                        {
                            temp = 0;
                            multiplier = 128;

                            while (multiplier >= 1)
                            {
                                temp = intmessage[bitmeter] | multiplier;
                                boolmessage[bytemeter++] = temp == intmessage[bitmeter] ? true : false;

                                multiplier /= 2;
                            }
                            bitmeter++;
                        }
                    }

                    //Changing bits of original image
                    Bitmap finalImage = new Bitmap(width, height);
                    bool[] boolwidth = new bool[15];  //Space reserved for value of width of 'image to hide'
                    bool[] boolheight = new bool[15]; //Space reserved for value of height of 'image to hide'

                    temp = 0;
                    bitmeter = 0;
                    multiplier = 16384;

                    while (multiplier >= 1)
                    {
                        temp = widthToHide | multiplier;
                        boolwidth[bitmeter++] = temp == widthToHide ? true : false;
                        multiplier /= 2;
                    }

                    temp = 0;
                    bitmeter = 0;
                    multiplier = 16384;

                    while (multiplier >= 1)
                    {
                        temp = heightToHide | multiplier;
                        boolheight[bitmeter++] = temp == heightToHide ? true : false;
                        multiplier /= 2;
                    }

                    bitmeter = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            getColor = bitmap.GetPixel(j, i);

                            red = getColor.R;
                            green = getColor.G;
                            blue = getColor.B;

                            if ((i == height - 1) && (j >= width - 10))
                            {
                                if (j == width - 5 || j == width - 10) bitmeter = 0;

                                //changing to '1': color | 1
                                //changing to '0': color & 254
                                if (j >= width - 10 && j < width - 5)
                                {
                                    newRed = boolheight[bitmeter++] ? red | 1 : red & 254;
                                    newGreen = boolheight[bitmeter++] ? green | 1 : green & 254;
                                    newBlue = boolheight[bitmeter++] ? blue | 1 : blue & 254;
                                    color = Color.FromArgb(newRed, newGreen, newBlue);
                                    finalImage.SetPixel(j, i, color);
                                }
                                else
                                {
                                    newRed = boolwidth[bitmeter++] ? red | 1 : red & 254;
                                    newGreen = boolwidth[bitmeter++] ? green | 1 : green & 254;
                                    newBlue = boolwidth[bitmeter++] ? blue | 1 : blue & 254;
                                    color = Color.FromArgb(newRed, newGreen, newBlue);
                                    finalImage.SetPixel(j, i, color);
                                }
                            }
                            else if (bitmeter >= boolmessage.Length)
                            {
                                color = Color.FromArgb(red, green, blue);
                                finalImage.SetPixel(j, i, color);
                            }
                            else
                            {
                                if (boolmessage.Length - bitmeter >= 3)
                                {
                                    newRed = boolmessage[bitmeter++] ? red | 1 : red & 254;
                                    newGreen = boolmessage[bitmeter++] ? green | 1 : green & 254;
                                    newBlue = boolmessage[bitmeter++] ? blue | 1 : blue & 254;
                                    color = Color.FromArgb(newRed, newGreen, newBlue);
                                    finalImage.SetPixel(j, i, color);
                                }
                                else if (boolmessage.Length - bitmeter == 2)
                                {
                                    newRed = boolmessage[bitmeter++] ? red | 1 : red & 254;
                                    newGreen = boolmessage[bitmeter++] ? green | 1 : green & 254;
                                    color = Color.FromArgb(newRed, newGreen, blue);
                                    finalImage.SetPixel(j, i, color);
                                }
                                else if (boolmessage.Length - bitmeter == 1)
                                {
                                    newRed = boolmessage[bitmeter++] ? red | 1 : red & 254;
                                    color = Color.FromArgb(newRed, green, blue);
                                    finalImage.SetPixel(j, i, color);
                                }
                            }
                        }
                    }

                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.ShowDialog();
                    finalImage.Save(dialog.FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    MessageBox.Show("Selected 'image to hide' is too big. Please select image which amount of pixels is smaller than or equal to " +
                    ((height * 3 * width / 8) - 5) + ".", "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select images.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void load_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            path = ofd.FileName;
        }

        private void loadToHide_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd2 = new OpenFileDialog();
            ofd2.ShowDialog();
            pathToHide = ofd2.FileName;
        }

        private void loadToDecode_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd3 = new OpenFileDialog();
            ofd3.ShowDialog();
            pathToDecode = ofd3.FileName;
        }

        private void decode_btn_Click(object sender, RoutedEventArgs e)
        {
            if (pathToDecode != null && pathToDecode != "")
            {
                int height, width, hiddenWidth = 0, hiddenHeight = 0;
                int bitmeter = 0, multiplier;
                Bitmap bitmap = new Bitmap(pathToDecode);
                Color color = new Color();

                height = bitmap.Height;
                width = bitmap.Width;

                //Getting height and width of hidden image
                int[] heightArray = new int[15];
                int[] widthArray = new int[15];

                for (int i = width - 10; i < width; i++)
                {
                    if (i == width - 10 || i == width - 5) bitmeter = 0;
                    if (i > width - 5)
                    {
                        color = bitmap.GetPixel(i, height - 1);
                        widthArray[bitmeter++] = (color.R | 1) == color.R ? 1 : 0;
                        widthArray[bitmeter++] = (color.G | 1) == color.G ? 1 : 0;
                        widthArray[bitmeter++] = (color.B | 1) == color.B ? 1 : 0;
                    }
                    else
                    {
                        color = bitmap.GetPixel(i, height - 1);
                        heightArray[bitmeter++] = (color.R | 1) == color.R ? 1 : 0;
                        heightArray[bitmeter++] = (color.G | 1) == color.G ? 1 : 0;
                        heightArray[bitmeter++] = (color.B | 1) == color.B ? 1 : 0;
                    }
                }

                multiplier = 16384;
                foreach (var v in heightArray)
                {
                    hiddenHeight += multiplier * v;
                    multiplier /= 2;
                }

                multiplier = 16384;
                foreach (var v in widthArray)
                {
                    hiddenWidth += multiplier * v;
                    multiplier /= 2;
                }


                //Getting last bits from image
                int[] getBitsArray = new int[hiddenHeight * hiddenWidth * 8];
                bitmeter = 0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        color = bitmap.GetPixel(j, i);

                        if ((hiddenHeight * hiddenWidth * 8) - bitmeter >= 3)
                        {
                            getBitsArray[bitmeter++] = (color.R | 1) == color.R ? 1 : 0;
                            getBitsArray[bitmeter++] = (color.G | 1) == color.G ? 1 : 0;
                            getBitsArray[bitmeter++] = (color.B | 1) == color.B ? 1 : 0;
                        }
                         else if ((hiddenHeight * hiddenWidth * 8) - bitmeter == 2)
                        {
                            getBitsArray[bitmeter++] = (color.R | 1) == color.R ? 1 : 0;
                            getBitsArray[bitmeter++] = (color.G | 1) == color.G ? 1 : 0;
                        }
                        else if ((hiddenHeight * hiddenWidth * 8) - bitmeter == 1)
                        {
                            getBitsArray[bitmeter++] = (color.R | 1) == color.R ? 1 : 0;
                        }
                        else
                        {
                            i = height;
                            j = width;
                        }
                    }
                }
                
                //Changing every 8 bits to values
                int[] getValuesArray = new int[hiddenHeight*hiddenWidth];
                int value = 0;
                bitmeter = 0;
                multiplier = 128;

                foreach (var v in getBitsArray)
                {
                    value += multiplier * v;
                    multiplier /= 2;
                    if (multiplier == 0)
                    {
                        getValuesArray[bitmeter++] = value;
                        multiplier = 128;
                        value = 0;
                    }
                }

                //Put values to bitmap
                Bitmap hiddenImage = new Bitmap(hiddenWidth, hiddenHeight);
                int h = 0, w = 0;
                foreach (var v in getValuesArray)
                {
                    color = Color.FromArgb(v, v, v);
                    hiddenImage.SetPixel(w++, h, color);

                    if (w == hiddenWidth) { h++; w = 0; }
                }

                SaveFileDialog dialog2 = new SaveFileDialog();
                dialog2.ShowDialog();
                hiddenImage.Save(dialog2.FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                MessageBox.Show("Please select image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}