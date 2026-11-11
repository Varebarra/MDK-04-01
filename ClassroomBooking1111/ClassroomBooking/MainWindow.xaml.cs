using ClassroomBooking.Models;
using ClassroomBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClassroomBooking;
using ClassroomBooking.Services;
using ClassroomBooking.Cofiguration;
using ClassroomBooking.Properties;
namespace ClassroomBooking
{
    public partial class MainWindow : Window
    {
        private readonly ConfigService configService = new ConfigService();
        private AppSettings settings;
        private readonly UserService userService = new UserService();
        private readonly RoomService roomService = new RoomService();
        private readonly BookingService bookingService = new BookingService();

        public MainWindow()
        {
            InitializeComponent();

            settings = configService.Load();

            Title = settings.AppTitle;
            StatusTextBlock.Text = settings.StatusMessage;

            UserComboBox.ItemsSource = userService.GetUsers();
            RoomComboBox.ItemsSource = roomService.GetRooms();
            BookingDatePicker.SelectedDate = DateTime.Today;

            RefreshBookings();
        }
       
        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(UserComboBox.SelectedItem is User user))
            {
                StatusTextBlock.Text = "Выберите пользователя.";
                return;
            }

            if (!(RoomComboBox.SelectedItem is Room room))
            {
                StatusTextBlock.Text = "Выберите аудиторию.";
                return;
            }

            if (!(BookingDatePicker.SelectedDate is DateTime date))
            {
                StatusTextBlock.Text = "Выберите дату.";
                return;
            }

            TimeSpan workStart;
            TimeSpan workEnd;

            if (!TimeSpan.TryParse(settings.WorkDayStart, out workStart) ||
                !TimeSpan.TryParse(settings.WorkDayEnd, out workEnd))
            {
                StatusTextBlock.Text = "Ошибка конфигурации рабочего времени.";
                return;
            }


            if (!TimeSpan.TryParse(StartTimeTextBox.Text, out var startTime) ||
                !TimeSpan.TryParse(EndTimeTextBox.Text, out var endTime))
            {
                StatusTextBlock.Text = "Введите время в формате ЧЧ:ММ.";
                return;
            }

            double duration =
    (endTime - startTime).TotalMinutes;

            if (duration < settings.MinBookingMinutes)
            {
                StatusTextBlock.Text =
                    "Минимальная длительность — " +
                    settings.MinBookingMinutes + " минут.";
                return;
            }


            if (startTime < workStart || endTime > workEnd)
            {
                StatusTextBlock.Text =
                    "Бронирование доступно с " +
                    settings.WorkDayStart + " до " +
                    settings.WorkDayEnd + ".";
                return;
            }



            if (startTime >= endTime)
            {
                StatusTextBlock.Text = "Время начала должно быть раньше времени окончания.";
                return;
            }

            var booking = new Booking
            {
                UserId = user.Id,
                RoomId = room.Id,
                Date = date,
                StartTime = startTime,
                EndTime = endTime
            };

            if (!room.IsAvailable && !settings.AllowUnavailableRooms)
            {
                StatusTextBlock.Text =
                    "Выбранная аудитория недоступна для бронирования.";
                return;
            }

            StatusTextBlock.Text = "Бронирование создано.";
            RefreshBookings();
        }

        private void RefreshBookings()
        {
            var rooms = roomService.GetRooms();
            var users = userService.GetUsers();

            var items = bookingService.GetBookings()
                .Select(x =>
                {
                    var user = users.FirstOrDefault(u => u.Id == x.UserId);
                    var room = rooms.FirstOrDefault(r => r.Id == x.RoomId);

                    return $"{x.Date:dd.MM.yyyy} | {x.StartTime:hh\\:mm}-{x.EndTime:hh\\:mm} | " +
                           $"{room?.Number} | {user?.Name}";
                })
                .ToList();

            BookingsListBox.ItemsSource = items;
        }
    }
}