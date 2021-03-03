import { Component, OnInit } from '@angular/core';
import { ImageWrapper } from 'src/ImageWrapper';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-mqtt',
  templateUrl: './mqtt.component.html',
  styleUrls: ['./mqtt.component.sass'],
})
export class MqttComponent implements OnInit {
  imageWrapper: ImageWrapper;
  imageUrl: string;
  width: number = 300;
  height: number = 400;

  private url: string = 'https://localhost:44316/mqtt/FromUrl?';

  constructor(private http: HttpClient) {
    this.imageUrl =
      'https://techcrunch.com/wp-content/uploads/2018/07/logo-2.png';
    this.imageWrapper = new ImageWrapper();
    this.imageWrapper.base64 =
      'iVBORw0KGgoAAAANSUhEUgAAAJwAAABZCAYAAADLnU1KAAAFSUlEQVR4Xu2dPXbVMBCF5TY11FQU1E7DDlgJPSuhZyXsgOa5pqCihjqtOSI48TGW7x35JxrpvlZjSXPny4ws2U4XQhgD8bv1fbgfBmgpu3WJkC7jSIUB6l+6QReBG8dXpc+z6vl13e8YhKp9nJx7Au7d3bdTHP7+8D6c1XeccOx/GL6eMvcrOu37D0HAHai0gNsWU8AdCNuUgZTh0qIKOAF3sALKcHMFtIa7FK//B1OGOzgAHtZwQ3//5HU/3FYVmNtYJUr1GfsRcFY1gb2AC0HAPUMCS+qnh7cUgp/vfqzabQG3t+8jtkWWmUsZjgp3tlF36/uxv/1M7pXthaJU4FIlkgFuK2NZIxFL6nD/JvS39VJu7a90ezrDpTIYcjCnpM4hR+PmbvyWBJw2fmcUTcFHgU+BZwXOAtuekirgUKo4p53OcKnhEYgW4Kyw7QFu7s8Zd6ls2dVd6oIsZg23BV0OcAji+RRzS+rZwP3d8khssczHFnDGTIpKrgW4OLQ1y5UMHAOdgHth4KzQXQWcRRZ2q0UbvxZV/9keneGmKbCZrkTgog/MulDAFQTcPNOhNeLe5+FYOCzysH2qpCZuGlJB35Ph0LWo/cq7VAtsynBpteiThjOBi9Nb9t9SSdVJg3FrBJW8rQcw0bYL2iLRGs6ae1/WHm78ri3i51NmgEBP/KagQ31fWVLZNdmkDWuvNdzBfwDWfTjr8FdlOBYgAbcdQTrDWUGY7EsFLgI0nQQwMFn21iy2ynC5ZCWuKxk45OryaCrnqV90vCXgUBSM7TUBt9zuQFIg2LTxixTMaK8NOBY6BjYBlwEUuqRU4NC8r2pXST1YaQG3LWhzwKF3GvbyJ+AwcDpp2EvZ7HoBh4Fr8p2GAxlTV0YFmgQOHT8ZNXwyjxkuhF+5lxPXvT65f2IKu0xe63Ndu/RbXCzgkJoCDilkahdwSC4BhxQytQs4JJeAQwqZ2gUckkvAIYVM7QIOySXgkEKmdg/AfQnxKa3H30fivwjM7VNiMP08XtsYcLknDcwLLlHO2oBjYLPAG4HTSQPIYewLLh6AWwK0lZkssPHQNZbhpn8Mwm78Lt8/QO8dlJzh1gBKAben7G6XVwGXzG9rL7t4BS6VrQScaYluNqbfaViWUc9ruK3SKODMDJkuoIBbW7N5BW6tNDLl0mozj4JK6rMaELjUDYJH4FLQWGFisqCAW098NHCpTzF4WcNtQSXgTFVxlzENHBpl67P5L/14EgIKtUffrdmR6bPJjV+0LYK+/TGBWCpwTOD32FhB/P8Pt7FtkdpPGvbANIfDChYz7pThdNKA6ufsW7ylr+FyTgfWTgnO27drLMOhkppiz8tdqoAjsseFJvCmQcA9KqAMdwyV2cCxw5d8ljr5wKy3GBtmzdf8TUNuSRVw2wrwgGoNx7JE2SnDoYc6BRwFEmtUC3Bb67gtLfCTvwKOZYmyqwk4K3QYttijgKNAYo1qA46FjoNNwLEc0XYegKOdOcWwsQyXe7TFai/gkFJ6iQYpZGoXcEiuxjKc9uEQEGe3C7hDFVaGQ3I2ChySRe3nKdDcBwkZKW99H+6HAZpebTeOY+i6Llw97iTEUeNGP1r4dWMrnrYQTQc+CjgHQappigKupmg68EXAOQhSTVMUcDVF04EvAs5BkGqaooCrKZoOfBFwDoJU0xQFXE3RdOCLgHMQpJqmKOBqiqYDXwScgyDVNEUBV1M0Hfgi4BwEqaYpCriaounAFwHnIEg1TVHA1RRNB74IOAdBqmmKAq6maDrwRcA5CFJNUxRwNUXTgS8CzkGQapriH0xlsUGu2eByAAAAAElFTkSuQmCC';
    this.imageWrapper.bytes = '32-33';
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

    this.http.post<ImageWrapper>(requestUrl, undefined).subscribe(
      (data) => {
        console.log(data);
        this.imageWrapper = data;
      },
      (err) => {
        this.imageWrapper.bytes = 'error';

        console.log(err);
      }
    );
  }
}
