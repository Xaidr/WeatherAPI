using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using WeatherAPI.Converters;

namespace WeatherAPI
{
	/// <summary>
	/// Главный класс
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Переменная, хранения текущего id города
		/// </summary>
		public double IdCity;

		/// <summary>
		/// Главный конструктор, инициализирующий элементы  
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			GetListCities();
			CbCities.Items.Add(new CityCities() { name = "Загрузка..."});
			CbCities.SelectedIndex = 0;
			GetWeather(524901);

			//Событие нажатия на кнопку "Update" запускает асинхронное обновление информации
			BtnUpdate.Click += (object sender, RoutedEventArgs e) =>
			{
				GetWeather(IdCity);
			};
			//Событие выбора элемента из списка запускает асинхронное обновление информации,
			//если элемент существует
			CbCities.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
			{
				if (((sender as ComboBox).SelectedItem as CityCities) != null)
					GetWeather(((sender as ComboBox).SelectedItem as CityCities).id);
			};
		}

		/// <summary>
		/// Асинхронный метод для получения "городов" из файла city.list.json
		/// и привязывает его к CbCities, устанавливая по умолчанию Москву
		/// </summary>
		public async void GetListCities()
		{
			string Data = "";
			try
			{
				using (StreamReader Sr = File.OpenText(@"..\..\Data\city.list.json"))
				{
					Data = await Sr.ReadToEndAsync();
				}
				Converters.RootCities Cities = JsonConvert.DeserializeObject<Converters.RootCities>(Data);
				Cities.city.Sort(delegate (CityCities x, CityCities y) {
					return x.name.CompareTo(y.name);
				});
				CityCities MoscowCity = Cities.city.Find(City => City.name.Contains("Moscow"));
				Dispatcher.Invoke(() =>
				{
					CbCities.Items.Clear();
					CbCities.ItemsSource = Cities.city;
					CbCities.SelectedItem = MoscowCity;
				});
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				MessageBox.Show("Ошибочка вышла, даже не знаю, что делать...");
			}
		}
		/// <summary>
		/// асинхронный метод создает запрос для API,
		/// декодирует JSON в необходимую информацию и
		/// обновляет TbTemp, TbWindSpeed и TbDescript
		/// </summary>
		/// <param name="IdCity">
		/// Задает необходимый id города для получения текущей погоды в нем
		/// </param>
		public async void GetWeather(double IdCity)
		{
			this.IdCity = IdCity;
			string _Temp;
			string _Wind;
			string _Description;

			try
			{
				WebRequest Request =
						WebRequest.Create("http://api.openweathermap.org/data/2.5/forecast?id=" + IdCity.ToString() +
											"&units=metric&appid=eb360b90b5c763be0b5b2c793926d239");
				Request.Method = "Post";
				WebResponse Response = await Request.GetResponseAsync();
				string Answer = "";
				using (Stream St = Response.GetResponseStream())
				{
					using (StreamReader Sr = new StreamReader(Response.GetResponseStream()))
					{
						Answer = await Sr.ReadToEndAsync();
					}
				}
				Response.Close();
				Converters.Root CurrentWeather = JsonConvert.DeserializeObject<Converters.Root>(Answer);
				_Temp = CurrentWeather.list[0].main.temp.ToString() + " Градусов";
				_Wind = CurrentWeather.list[0].wind.speed.ToString() + " м/с";
				_Description = CurrentWeather.list[0].weather[0].description;
				Dispatcher.Invoke(() => {
					TbTemp.Text = _Temp;
					TbWindSpeed.Text = _Wind;
					TbDescript.Text = _Description;
				});
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				MessageBox.Show("Ошибочка вышла, даже не знаю, что делать...");
			}
		}
	}
}
