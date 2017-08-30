import { Injectable, NgZone  } from '@angular/core';
import { Geolocation, Geoposition, BackgroundGeolocation } from 'ionic-native';
import {Http,Headers} from '@angular/http';
import 'rxjs/add/operator/map';

/*
  Generated class for the LocationTracker provider.

  See https://angular.io/docs/ts/latest/guide/dependency-injection.html
  for more info on providers and Angular 2 DI.
*/
@Injectable()
export class LocationTracker {

  private watch: any;    
  private lat: number = 0;
  private lng: number = 0;

  

  constructor(public zone: NgZone, public http: Http) {}

  startTracking(id: string, url: string) {
  console.log("locationing");
  this.lat = 0;
  this.lng = 0;
 
  let config = {
    desiredAccuracy: 0,
    stationaryRadius: 20,
    distanceFilter: 10, 
    debug: true,
    interval: 2000 
  };

BackgroundGeolocation.configure(config).then((location) => {
    console.log('BackgroundGeolocation:  ' + location.latitude + ',' + location.longitude);
 
    // Run update inside of Angular's zone
    this.zone.run(() => {
      this.lat = location.latitude;
      this.lng = location.longitude;
      
    });
    }).catch((err) => console.log("Error ", err));
 
  // Turn ON the background-geolocation system.
  BackgroundGeolocation.start();
 
 
  // Foreground Tracking
 
  let options = {
    frequency: 3000, 
    enableHighAccuracy: true
  };
 
  this.watch = Geolocation.watchPosition(options).filter((p: any) => p.code === undefined).subscribe((position: Geoposition) => {
    // Run update inside of Angular's zone
    this.zone.run(() => {
      this.lat = position.coords.latitude;
      this.lng = position.coords.longitude;
      if(this.lat != 0 && this.lng != 0)
      {
        var link = url + 'Login/UpdateLocation';
        var data = JSON.stringify({
            id: id,
            locationX: this.lat,
            locationY: this.lng
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post(link, data, { headers: headers })
            .subscribe(data => {
            }, error => {
              console.log("Set location error");
            });
        console.log('stopTracking');
        BackgroundGeolocation.stop();
      }
    });
  });
  }
}
