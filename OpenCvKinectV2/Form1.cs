using System;
using System.Drawing;
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
        /// 最新の深度画像
        /// </summary>
        Mat depthImage;

        /// <summary>
        /// 出力用に処理後の画像
        /// </summary>
        Mat colorOutputImage;

        /// <summary>
        /// 出力用に処理後の画像
        /// </summary>
        Mat depthOutputImage;

        /// <summary>
        /// カラー画像を入れるBitmap
        /// </summary>
        Bitmap colorBitmap;

        /// <summary>
        /// 深度画像を入れるBitmap
        /// </summary>
        Bitmap depthBitmap;


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
                FrameSourceTypes.Color | FrameSourceTypes.Depth     // カラーと深度
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

            // カラー画像Bitmapの作成とPictureBosへの割り当て
            this.colorBitmap = this.colorOutputImage.ToBitmap();
            this.pictureBoxColor.Image = this.colorBitmap;


            // 深度画像の準備
            FrameDescription depthFrameDescription = this.kinect.DepthFrameSource.FrameDescription;
            this.depthImage = new Mat(
                depthFrameDescription.Height,
                depthFrameDescription.Width,
                MatType.CV_16UC1                     // 16ビット×1チャンネル
                );
            this.depthImage.SetTo(Scalar.All(0));    // 画像全体を黒（ゼロ）に塗りつぶし

            // 表示用深度画像の準備。深度の値は直接表示に向かないため。
            this.depthOutputImage = new Mat(
                depthFrameDescription.Height,
                depthFrameDescription.Width,
                MatType.CV_8UC1                     // 8ビット×1チャンネル
                );
            this.depthOutputImage.SetTo(Scalar.All(0));    // 画像全体を黒（ゼロ）に塗りつぶし

            // 深度画像Bitmapの作成とPictureBoxへの割り当て
            this.depthBitmap = this.depthOutputImage.ToBitmap();
            this.pictureBoxDepth.Image = this.depthBitmap;
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
                    UpdatePictureBox(this.colorOutputImage);    // this.colorImage を渡すと元のカラー画像が見られる
                }
            }

            // 深度画像取得時の処理
            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    // OpenCVの画像にKinectの深度画像を複製
                    depthFrame.CopyFrameDataToIntPtr(
                        this.depthImage.Data,
                        (uint)(this.depthImage.Total() * this.depthImage.ElemSize())
                        );

                    // 0mm～8000mmの16ビット深度画像を256階調に変換
                    this.depthImage.ConvertTo(this.depthOutputImage, this.depthOutputImage.Type(), 255.0 / 8000.0);

                    // 深度画像からPictureBoxへの描画
                    UpdateDepthPictureBox(this.depthOutputImage);   // ここでは this.depthImage は渡せない。色深度が異なる。
                }
            }
        }

        /// <summary>
        /// カラー表示PictureBoxの表示を更新
        /// <param name="image">表示画像。colorBitmapと同じサイズ・色深度でなければならない</param>
        /// </summary>
        private void UpdatePictureBox(Mat image)
        {
            // 指定された画像でBitmapを更新
            image.ToBitmap(this.colorBitmap);

            // PictureBoxの描画を要求
            this.pictureBoxColor.Invalidate();
        }

        /// <summary>
        /// 深度表示PictureBoxを更新
        /// <param name="image">表示画像。depthBitmapと同じサイズ・色深度でなければならない
        /// Kinectから得られる16bitの深度ではないので注意。</param>
        /// </summary>
        private void UpdateDepthPictureBox(Mat image)
        {
            // 指定された画像でBitmapを更新
            image.ToBitmap(this.depthBitmap);

            // PictureBoxの描画を要求
            this.pictureBoxDepth.Invalidate();
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
