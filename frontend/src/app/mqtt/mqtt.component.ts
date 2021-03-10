import { Component, OnInit } from '@angular/core';
import { EpaperImage } from 'src/EpaperImage';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-mqtt',
  templateUrl: './mqtt.component.html',
  styleUrls: ['./mqtt.component.sass'],
})
export class MqttComponent implements OnInit {
  epaperImage!: EpaperImage;
  imageUrl: string;
  width: number = 400;
  height: number = 300;
  asBytes: boolean = false;

  private url: string = 'https://localhost:44316/image/FromUrlAsBytes?';
  constructor(private http: HttpClient) {
    this.imageUrl =
      'https://techcrunch.com/wp-content/uploads/2018/07/logo-2.png';
  }

  ngOnInit() {}

  getNewImage() {
    let requestUrl =
      this.url +
      'width=' +
      this.width +
      '&height=' +
      this.height +
      '&imageUrl=' +
      this.imageUrl;
    requestUrl = 'https://localhost:44316/image/WeatherImage';
    this.http.get<EpaperImage>(requestUrl, undefined).subscribe(
      (data) => {
        console.log(data);
        this.epaperImage = data;
      },
      (err) => {
        this.epaperImage.bytes = 'error';

        console.log(err);
      }
    );
  }
}
