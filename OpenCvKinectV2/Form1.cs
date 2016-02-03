using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OpenCvKinectV2
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// KinectSensorのインスタンス
        /// </summary>
        KinectSensor kinect;

        /// <summary>
        /// 最新のカラー画像
        /// </summary>
        Mat colorImage;

        /// <summary>
        /// 出力用に処理後の画像
        /// </summary>
        Mat colorOutputImage;

        /// <summary>
        /// PictureBoxで表示するBitmap
        /// </summary>
        Bitmap colorBitmap;


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ウィンドウが開いたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // KinectSensorのインスタンスを取得
            this.kinect = KinectSensor.GetDefault();

            // カラー、深度等をまとめて取得するイベントハンドラを設定
            MultiSourceFrameReader multiSourceFrameReader = this.kinect.OpenMultiSourceFrameReader(
                FrameSourceTypes.Color      // 今回はカラーのみ
                );
            multiSourceFrameReader.MultiSourceFrameArrived += FrameArrived;

            // 画像の準備
            InitializeImage();

            // Kinect利用開始
            this.kinect.Open();
        }

        /// <summary>
        /// 画像を新規作成
        /// </summary>
        private void InitializeImage()
        {
            // カラー画像の準備
            FrameDescription colorFrameDescription = this.kinect.ColorFrameSource.FrameDescription;
            this.colorImage = new Mat(
                colorFrameDescription.Height,
                colorFrameDescription.Width,
                MatType.CV_8UC4                     // 8ビット×4チャンネル分
                );
            this.colorImage.SetTo(Scalar.All(255)); // 最初は画像全体を白色に塗りつぶし

            // 同じサイズ・深度で出力用画像を作成
            this.colorOutputImage = this.colorImage.Clone();

            // カラー画像Bitmapの作成
            this.colorBitmap = this.colorOutputImage.ToBitmap();

            // PictureBoxへBitmapを割り当て
            this.pictureBoxColorFrame.Image = this.colorBitmap;
        }

        /// <summary>
        /// Kinectのデータ取得時に呼ばれる処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            // カラー画像取得時の処理
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    // OpenCVの画像にKinectのカラー画像を複製
                    colorFrame.CopyConvertedFrameDataToIntPtr(
                        this.colorImage.Data,   // カラー画像Matのデータ部分ポインタ
                        (uint)(this.colorImage.Total() * this.colorImage.ElemSize()),   // 全画素数 × 一画素のバイト数
                        ColorImageFormat.Bgra   // RGBでなくBGRAの順番
                        );

                    // これでOpenCVでの画像処理ができる
                    // （例としてRGBごとの閾値処理）
                    Cv2.Threshold(this.colorImage, this.colorOutputImage, 127.0, 255.0, ThresholdTypes.Binary);

                    // PictureBoxへ描画
                    UpdatePictureBox(this.colorOutputImage);
                }
            }
        }

        /// <summary>
        /// PictureBoxの表示を更新
        /// <param name="image">表示画像。colorBitmapと同じサイズ・色深度でなければならない</param>
        /// </summary>
        private void UpdatePictureBox(Mat image)
        {
            // 指定された画像でBitmapを更新
            image.ToBitmap(this.colorBitmap);

            // PictureBoxの描画を要求
            this.pictureBoxColorFrame.Invalidate();
        }

        /// <summary>
        /// ウィンドウが閉じられたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Kinect利用終了
            this.kinect.Close();
        }
    }
}
