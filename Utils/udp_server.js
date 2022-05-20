let data = {};
let deviceList = [];
let timerText = '';
let timerActive = false;
let deviceNameCPR = 'PC245-25';

const dgram = require('dgram');
const server = dgram.createSocket('udp4');

server.on('message', function(msg, senderInfo){
  //console.log('server got message:' + msg);
  let message = JSON.parse( msg.toString() );
    
  if(message.type === 'get') {
      let deviceName = message.device;
      let otherDevice = deviceName === deviceList[0] ? deviceList[1] : deviceList[0];
      
      try {
          
          let sendMessage = '{"objects":[';
          
          for( const [key,value] of Object.entries(data[otherDevice]) ){
              sendMessage += JSON.stringify(value) + ',';
          }
          sendMessage = sendMessage.substr(0, sendMessage.length - 1) + '], "timerText": "' + timerText + '", "timerActive":' + timerActive + '}';
          server.send(sendMessage,senderInfo.port,senderInfo.address);
          
      } catch(e) {
          //console.log(e)
      }

  
  } else if(message.type === 'set'){
      
      let deviceName = message.device;
      
      if(deviceName === deviceNameCPR){
          timerActive = message.timerActive ?? false;
      }
      
      if(message.timerText) timerText = message.timerText;
      
      let array = message.objects;
      
      if(!data[deviceName]){
          deviceList.push(deviceName);
          data[deviceName] = {};
      }
      
      array.forEach( (obj) => {
          data[deviceName][obj.name] = obj;
      });
      
  }
  
});

server.on('listening', function(){
	const address = server.address();
	console.log('server is listening on ' + address.address + ':' + address.port);
});

setInterval( () => {
    //console.log(data, timerText, timerActive)
}, 1000) 

server.bind({
	address: '192.168.1.1',
	port: 4000,
});