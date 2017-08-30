import { Component, NgZone } from '@angular/core';
import { NavController, NavParams, MenuController, AlertController, ToastController, LoadingController } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import { ChatViewPage } from "../chat-view/chat-view"
import {Http,Headers} from '@angular/http';

import { LocationTracker } from '../../providers/location-tracker';
/*
  Generated class for the FriendList page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-friend-list',
  templateUrl: 'friend-list.html'
})
export class FriendListPage {
  id: any;

  systemInformation: Array<{
    senderId: string;
    senderNickName: string;
    content: string;
    time: string;
    InforClass: string;
  }>

  firendInfor: Array<{
    friend: string,
    friendNickName: string,
    chatId: string,
    sendTime: string,
    lastMessage: string,
    loginStatus: string
  }>;
  
  setInterval1: any;
  setInterval2: any;
  setInterval3: any;
  url: any;

  flagFriend: boolean = true;
  flagSystem: boolean = true;
  
  
  constructor(public navCtrl: NavController,
   public navParams: NavParams,
    public menu: MenuController,
     private sharedata: ShareData,
     public http: Http,
     public alertCtrl: AlertController,
     public toastCtrl: ToastController,
     public zone: NgZone,
     public zone2: NgZone,
     public zone3: NgZone,
     public locationTracker: LocationTracker,
     public loadingCtrl: LoadingController) {
       this.url = sharedata.geturl();
    menu.swipeEnable(true);
    this.id = sharedata.getId();
    

    var data = JSON.stringify({
      id: this.id
    });

    this.zone.runOutsideAngular(() => this.setInterval1 = setInterval( () => this.GetSystemList(data), 5000));
    this.zone2.runOutsideAngular( () => this.setInterval2 = setInterval( () =>this.GetFriendList(data), 3000));
    this.zone3.runOutsideAngular( () => this.setInterval3 = setInterval( () =>this.UpdateLocation(), 15000));
  }

  UpdateLocation(){
    var flag = this.sharedata.getLogOut3();
    if( flag == 1){
      clearInterval(this.setInterval3);
      this.sharedata.setLogOut3(0);
    }
    this.locationTracker.startTracking( this.id, this.url)
  }

  GetFriendList(data){
    if(this.flagFriend){
      this.zone2.run( () => this.flagFriend = false);
      console.log("friendCycle");
      var flag = this.sharedata.getLogOut2();
      if( flag == 1){
        clearInterval(this.setInterval2);
        this.sharedata.setLogOut2(0);
      }
      
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post( this.url + 'Login/GetFriendList', data, { headers: headers })
          .subscribe(data => {
            this.zone2.run(() => {
              this.flagFriend = true;
              this.firendInfor = data.json();
              flag = this.sharedata.getLogOut2();
            })
          }, error => {
             this.zone2.run( () => this.flagFriend = true);
              let toast = this.toastCtrl.create({
                  message: 'Ooop, connection seem unstable.',
                  duration: 3000
                });
              toast.present();
      });
    }
  }

  GetSystemList(data){
    if(this.flagSystem){
      this.zone.run(() => this.flagSystem = false);
      var flag = this.sharedata.getLogOut1();
      if( flag == 1){
        clearInterval(this.setInterval1);
        this.sharedata.setLogOut1(0);
      }
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post( this.url + 'Login/GetSystemList', data, { headers: headers })
          .subscribe(data => {
            this.zone.run(() => {
              this.flagSystem = true
              this.systemInformation = data.json();
            })
          }, error => {
              this.zone.run( ()=> this.flagSystem = true);
              let toast = this.toastCtrl.create({
                  message: 'Ooop, connection seem unstable.',
                  duration: 3000
                });
              toast.present();
      });
    }
  }

  StartChat(friendInfor){
    this.navCtrl.push(ChatViewPage, {chatId: friendInfor.chatId, nickName: friendInfor.friendNickName});
    
  }

  DeleteFriend(friend){
    let confirm = this.alertCtrl.create({
      title: 'Warning',
      message: "Are you sure delete " + friend.nickName + " from your friend list?",
      buttons: [
        {
          text: 'Confirm',
          handler: () => { this.ConfirmDelete( this.id, friend.id ) }
        },
        {
          text: 'Cancel',
        }
      ]
    });
    confirm.present();
  }

  ConfirmDelete(id, friendId){
    let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Login/DeleteRelationship';
    var data = JSON.stringify({
      id: this.id,
      friendId: friendId
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
          loader.dismiss();
        }, error => {
          loader.dismiss();
            let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
    });
  }

  GetDetail(systemInformation){
    if(systemInformation.inforClass == "0"){
      this.FriendAddView(systemInformation);
    }
    else{
      this.SystemInfor(systemInformation);
    }
  }

  FriendAddView(systemInformation){
    let confirm = this.alertCtrl.create({
      title: 'Friend Apply    ' + systemInformation.time,
      subTitle: 'From' + systemInformation.senderId + " " + systemInformation.nickName,
      message: systemInformation.content,
      buttons: [
        {
          text: 'Accept',
          handler: () => { this.AcceptRequire( systemInformation.senderId, systemInformation ) }
        },
        {
          text: 'Reject',
          handler: () => { this.RejectRequire( systemInformation.senderId, systemInformation )}
        },
        {
          text: 'Cancel',
        }
      ]
    });
    confirm.present();
  }

  AcceptRequire( senderId, systemInformation){
    var link = this.url + 'Login/CreateNewRelation';
    var data = JSON.stringify({
      id: this.id,
      senderId: senderId
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
          var i = this.systemInformation.indexOf(systemInformation);
          this.systemInformation.splice(i, 1);
        }, error => {
            let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
    });
  }

  RejectRequire( senderId, systemInformation ){
    var link = this.url + 'Login/RejectRequire';
    var data = JSON.stringify({
      id: this.id,
      senderId: senderId
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
        }, error => {
            let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
    });
  }

  SystemInfor(systemInformation){
      let confirm = this.alertCtrl.create({
      title: 'Friend Apply    ' + systemInformation.time,
      subTitle: 'From' + systemInformation.senderId + " " + systemInformation.nickName,
      message: systemInformation.content,
      buttons: [
        {
          text: 'Confirm',
          handler: () => { this.ReadedInfor(systemInformation.senderId, systemInformation);}
        }]
      });
      confirm.present();
  }

  ReadedInfor( senderId, systemInformation ){
    var link = this.url + 'Login/ReadedInfor';
    var data = JSON.stringify({
      id: this.id,
      senderId: senderId
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
            var i = this.systemInformation.indexOf(systemInformation);
            this.systemInformation.splice(i, 1);
        },
         error => {
            let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
    });
  }
}
