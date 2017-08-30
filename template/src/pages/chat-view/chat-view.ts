import { Component, NgZone, ViewChild } from '@angular/core';
import { NavController, NavParams, MenuController, ToastController, LoadingController, Content } from 'ionic-angular';
import { ShareData } from "../sharedata/sharedata"
import {Http, Headers} from '@angular/http';

/*
  Generated class for the ChatView page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-chat-view',
  templateUrl: 'chat-view.html'
})
export class ChatViewPage {
  @ViewChild(Content) content: Content;

  chatId = '';
  id = '';
  chat_input = '';
  nickName = '';
  message: Array<{
    nickName: string;
    time: string;
    content: string;
  }>

  tempMessage: Array<{
    nickName: string;
    time: string;
    content: string;
  }>

  setInterval: any;
  url: any;
  callback: any;

  updateTime: string;
  flag: boolean;

  constructor(public navCtrl: NavController, 
  public navParams: NavParams, 
  public menu: MenuController, 
  public sharedata: ShareData,
  public http: Http,
  public toastCtrl: ToastController,
  public zone: NgZone,
  public loadingCtrl: LoadingController) {
    this.callback = this.navParams.get("callback")
    this.url = sharedata.geturl();
    this.menu.swipeEnable(false);
    this.chatId = navParams.get('chatId');
    this.id = sharedata.getId();
    this.nickName = navParams.get('nickName');
    this.flag = true;
    //this.HistoryReview();
  }

  ionViewWillEnter() {
      console.log("newPage");
      this.HistoryReview();
    }

  Send(msg){
    if(msg != ''){
      var link = this.url + 'Chat/sendMessage';
      var data = JSON.stringify({
        chatId: this.chatId,
        id: this.id,
        content: msg
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post(link, data, { headers: headers })
          .subscribe(data => {
            this.zone.run( () => {this.chat_input = ''});
          }, error => {
              let toast = this.toastCtrl.create({
              message: 'Ooop, connection seem unstable.',
              duration: 3000
            });
          toast.present();
      });
    }
  }


  HistoryReview(){
    let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();
    var link = this.url + 'Chat/review';
      var data = JSON.stringify({
        chatId: this.chatId
      });
      var headers = new Headers();
      headers.append('Content-Type', 'application/json');
          this.http.post(link, data, { headers: headers })
          .subscribe(data => {
            loader.dismiss();
            this.message = data.json();
            this.AutoScroll();
            this.zone.runOutsideAngular( () => {
              this.setInterval = setInterval(() => this.NewMessage(), 1000);
               setTimeout( () => this.content.scrollToBottom(300), 500);
              });
          }, error => {
              loader.dismiss();
              let toast = this.toastCtrl.create({
              message: 'Ooop, connection seem unstable.',
              duration: 3000
            });
          toast.present();
      });
  }

  NewMessage(){
    console.log("check");
    if(this.flag){
      console.log("test");
      this.zone.run( () => this.flag = false);
      var i = this.message.length - 1;
      if( i < 0)
        this.updateTime = "2017/4/22 19:00:00";
      else
        if(this.message[i] == null)
          if(i == 0)
            this.updateTime = "2017/4/22 19:00:00";
          else
            this.updateTime = this.message[i - 1].time;
        else
          this.updateTime = this.message[i].time;
        var data = JSON.stringify({
          chatId: this.chatId,
          time: this.updateTime
        });
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
            this.http.post( this.url + 'Chat/newMessage' , data, { headers: headers })
            .subscribe(data => {
                console.log("working");
                this.tempMessage = data.json();
                this.zone.run( () => this.flag = true);
                if(this.tempMessage.length != 0)
                {
                  for(var i = 0; i < this.tempMessage.length; i++){
                    if(this.message[this.message.length - 1].time == this.tempMessage[i].time)
                      if(this.message[this.message.length - 1].content == this.tempMessage[i].content && this.message[this.message.length - 2].nickName == this.tempMessage[i].nickName)
                        continue;
                    
                    if(this.message[this.message.length - 2].time == this.tempMessage[i].time)
                      if(this.message[this.message.length - 2].content == this.tempMessage[i].content && this.message[this.message.length - 2].nickName == this.tempMessage[i].nickName)
                        continue;
                    this.zone.run( () => {
                      this.message.push(this.tempMessage[i]); 
                      this.AutoScroll(); 
                      setTimeout( () => this.content.scrollToBottom(300), 500);
                    });
                  }
                }
                
            }, error => {
              this.zone.run( () => this.flag = true);
            })
      }
  }

  AutoScroll(){
    var element = document.getElementById("bottomFlag");
    setTimeout( ()=> {element.scrollIntoView}, 10);
  }

  Back(){
    this.menu.swipeEnable(true);
    clearInterval(this.setInterval)
    this.navCtrl.pop();
  }

}
