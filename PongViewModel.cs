using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia;
using System.Threading;
using Avalonia.Threading;

namespace Pong
{
    internal class PongViewModel : ReactiveObject
    {
        public bool GameStarted;
        Timer computerTimer;
        Timer ballTimer;
        Timer playerTimer;
        public PongViewModel()
        {
            _PlayerY = _ComputerY = (field_Height - paddle_Height) / 2;
            _Ball = new Vector(fieldBallOffset / 2, fieldBallOffset / 2);

            Random randomErrror = new Random();
            computerTimer = new Timer(ComputerTimer, new object(), 5, 5);
            void ComputerTimer(object sender)
            {
                if (GameStarted)
                {
                    if (Ball.Y - ball_Size > ComputerY)
                    {
                        ComputerDown = randomErrror.Next(4) % 2 != 0;
                        ComputerUp = false;
                    }
                    else if (Ball.Y < ComputerY)
                    {
                        ComputerDown = false;
                        ComputerUp = randomErrror.Next(4) % 2 != 0;
                    }
                    else
                    {
                        ComputerUp = ComputerDown = false;
                    }

                    if (ComputerUp)
                        Dispatcher.UIThread.Post(() => ComputerY -= paddle_Speed);
                    else if (ComputerDown)
                        Dispatcher.UIThread.Post(() => ComputerY += paddle_Speed);
                }
            }

            ballTimer = new Timer(BallTimer, new object(), 5, 5);
            void BallTimer(object sender)
            {
                if (GameStarted)
                {
                    // Bounds
                    var ballUpdatedX = Math.Max(0, Math.Min(Ball.X + ball_Speed_Left, field_Width));
                    var ballUpdatedY = Math.Max(0, Math.Min(Ball.Y + ball_Speed_Top, fieldBallOffset));
                    
                    // Bounce on Top or Bottom Border
                    if (ballUpdatedY == 0 || ballUpdatedY == fieldBallOffset)
                        ball_Speed_Top *= -1;

                    // Hit player paddle
                    if (ballUpdatedX <= paddle_Width)
                    {
                        if (ballUpdatedY + ball_Size >= PlayerY
                                && ballUpdatedY <= PlayerY + (paddle_Height / 4))
                        {
                            ball_Speed_Left *= -1;
                            ball_Speed_Top += 2;
                            ballUpdatedX += paddle_Width - ballUpdatedX;
                        }
                        else if (ballUpdatedY + ball_Size >= PlayerY
                                && ballUpdatedY <= PlayerY + ((paddle_Height / 4) * 2))
                        {
                            ball_Speed_Left *= -1;
                            ballUpdatedX += paddle_Width - ballUpdatedX;
                        }   
                        else if (ballUpdatedY + ball_Size >= PlayerY
                                && ballUpdatedY <= PlayerY + paddle_Height - 1)
                        {
                            ball_Speed_Left *= -1;
                            ball_Speed_Top -= 2;
                            ballUpdatedX += paddle_Width - ballUpdatedX;
                        }
                    }

                    // Hit computer paddle
                    if (ballUpdatedX >= computerPaddleOffset - paddle_Width)
                    {
                        if (ballUpdatedY + ball_Size >= ComputerY
                                && ballUpdatedY <= ComputerY + (paddle_Height / 4))
                        {
                            ball_Speed_Left *= -1;
                            ball_Speed_Top += 2;
                            ballUpdatedX -= ballUpdatedX - (computerPaddleOffset - paddle_Width);
                        }
                        else if (ballUpdatedY + ball_Size >= ComputerY
                                && ballUpdatedY <= ComputerY + ((paddle_Height / 4) * 2))
                        {
                            ball_Speed_Left *= -1;
                            ballUpdatedX -= ballUpdatedX - (computerPaddleOffset - paddle_Width);
                        }
                        else if (ballUpdatedY + ball_Size >= ComputerY
                                && ballUpdatedY <= ComputerY + paddle_Height)
                        {
                            ball_Speed_Left *= -1;
                            ball_Speed_Top -= 2;
                            ballUpdatedX -= ballUpdatedX - (computerPaddleOffset - paddle_Width);
                        }
                    }

                    // Score Computer
                    if (ballUpdatedX == 0)
                    {
                        ballUpdatedY = ballUpdatedX = fieldBallOffset / 2;
                        ball_Speed_Top = 0;
                        ball_Speed_Left *= -1;
                        ComputerScore += 1;
                    }
                    // Score Player
                    else if (ballUpdatedX == field_Width)
                    {
                        ballUpdatedY = ballUpdatedX = fieldBallOffset / 2;
                        ball_Speed_Top = 0;
                        ball_Speed_Left *= -1;
                        PlayerScore += 1;
                    }

                    // Post Updated position
                    Dispatcher.UIThread.Post(() => Ball = new(ballUpdatedX, ballUpdatedY));
                    Dispatcher.UIThread.Post(() => Score = $"{PlayerScore} | {ComputerScore}");
                }
            }

            playerTimer = new Timer(PlayerTimer, new object(), 5, 5);
            void PlayerTimer(object sender)
            {
                if (PlayerUp)
                    Dispatcher.UIThread.Post(() => PlayerY -= paddle_Speed);
                else if (PlayerDown)
                    Dispatcher.UIThread.Post(() => PlayerY += paddle_Speed);
            }
        }

        public void Pause()
        {
            GameStarted = false;
            if (PlayerScore + ComputerScore > 0)
                Dispatcher.UIThread.Post(() => Score = "Paused"); 
            else
                Dispatcher.UIThread.Post(() => Score = "Use ↑ or ↓ arrow keys to start");
        }

        public int field_Height { get => 800; }
        public int field_Width { get => 800; }
        public int fieldPaddleOffset { get => field_Height - paddle_Height; }
        public int fieldBallOffset { get => field_Height - ball_Size; }
        public int computerPaddleOffset { get => field_Width - paddle_Width; }

        const int paddle_Speed = 10;

        public int paddle_Height { get => 80; }
        public int paddle_Width { get => 20; }

        int ball_Speed_Top = 0;
        int ball_Speed_Left = -5;
        public int ball_Size { get => 20; }

        public bool PlayerUp;
        public bool PlayerDown;

        public bool ComputerUp;
        public bool ComputerDown;

        int PlayerScore;
        int ComputerScore;
        string _Score; // "0 | 0";
        public string Score
        {
            get => _Score;
            set => this.RaiseAndSetIfChanged(ref _Score, value);
        }

        int _PlayerY;
        public int PlayerY
        {
            get => _PlayerY;
            set => this.RaiseAndSetIfChanged(ref _PlayerY, Math.Max(0, Math.Min(fieldPaddleOffset, value)));
        }

        int _ComputerY;
        public int ComputerY
        {
            get => _ComputerY;
            set => this.RaiseAndSetIfChanged(ref _ComputerY, Math.Max(0, Math.Min(fieldPaddleOffset, value)));
        }

        Vector _Ball;
        public Vector Ball
        {
            get => _Ball;
            set => this.RaiseAndSetIfChanged(ref _Ball, value);
        }
    }
}
