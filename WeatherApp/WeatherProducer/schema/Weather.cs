// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.11.1
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace WeatherProducer.AvroSpecific
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.11.1")]
	public partial class Weather : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""Weather"",""namespace"":""WeatherProducer.AvroSpecific"",""fields"":[{""name"":""id"",""type"":""string""},{""name"":""city"",""type"":""string""},{""name"":""latitude"",""type"":""double""},{""name"":""longitude"",""type"":""double""},{""name"":""generationtime_ms"",""type"":""double""},{""name"":""utc_offset_seconds"",""type"":""double""},{""name"":""timezone"",""type"":""string""},{""name"":""timezone_abbreviation"",""type"":""string""},{""name"":""elevation"",""type"":""double""},{""name"":""current_weather"",""type"":{""type"":""record"",""name"":""CurrentWeather"",""namespace"":""WeatherProducer.AvroSpecific"",""fields"":[{""name"":""temperature"",""type"":""double""},{""name"":""windspeed"",""type"":""double""},{""name"":""winddirection"",""type"":""double""},{""name"":""weathercode"",""type"":""int""},{""name"":""time"",""type"":""string""}]}}]}");
		private string _id;
		private string _city;
		private double _latitude;
		private double _longitude;
		private double _generationtime_ms;
		private double _utc_offset_seconds;
		private string _timezone;
		private string _timezone_abbreviation;
		private double _elevation;
		private WeatherProducer.AvroSpecific.CurrentWeather _current_weather;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return Weather._SCHEMA;
			}
		}
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}
		public string city
		{
			get
			{
				return this._city;
			}
			set
			{
				this._city = value;
			}
		}
		public double latitude
		{
			get
			{
				return this._latitude;
			}
			set
			{
				this._latitude = value;
			}
		}
		public double longitude
		{
			get
			{
				return this._longitude;
			}
			set
			{
				this._longitude = value;
			}
		}
		public double generationtime_ms
		{
			get
			{
				return this._generationtime_ms;
			}
			set
			{
				this._generationtime_ms = value;
			}
		}
		public double utc_offset_seconds
		{
			get
			{
				return this._utc_offset_seconds;
			}
			set
			{
				this._utc_offset_seconds = value;
			}
		}
		public string timezone
		{
			get
			{
				return this._timezone;
			}
			set
			{
				this._timezone = value;
			}
		}
		public string timezone_abbreviation
		{
			get
			{
				return this._timezone_abbreviation;
			}
			set
			{
				this._timezone_abbreviation = value;
			}
		}
		public double elevation
		{
			get
			{
				return this._elevation;
			}
			set
			{
				this._elevation = value;
			}
		}
		public WeatherProducer.AvroSpecific.CurrentWeather current_weather
		{
			get
			{
				return this._current_weather;
			}
			set
			{
				this._current_weather = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.id;
			case 1: return this.city;
			case 2: return this.latitude;
			case 3: return this.longitude;
			case 4: return this.generationtime_ms;
			case 5: return this.utc_offset_seconds;
			case 6: return this.timezone;
			case 7: return this.timezone_abbreviation;
			case 8: return this.elevation;
			case 9: return this.current_weather;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.id = (System.String)fieldValue; break;
			case 1: this.city = (System.String)fieldValue; break;
			case 2: this.latitude = (System.Double)fieldValue; break;
			case 3: this.longitude = (System.Double)fieldValue; break;
			case 4: this.generationtime_ms = (System.Double)fieldValue; break;
			case 5: this.utc_offset_seconds = (System.Double)fieldValue; break;
			case 6: this.timezone = (System.String)fieldValue; break;
			case 7: this.timezone_abbreviation = (System.String)fieldValue; break;
			case 8: this.elevation = (System.Double)fieldValue; break;
			case 9: this.current_weather = (WeatherProducer.AvroSpecific.CurrentWeather)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
