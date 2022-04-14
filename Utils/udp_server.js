let data = {};
let deviceList = [];
let timerText = '';
let timerActive = false;

const dgram = require('dgram');
const server = dgram.createSocket('udp4');

server.on('message', function(msg, senderInfo){
  //console.log('server got message:' + msg);
  let message = msg.toString();
    
  if(message.includes('get')) {
      let m = message.split('___');
      let deviceName = m[1];
      let otherDevice = deviceName === deviceList[0] ? deviceList[1] : deviceList[0];
      
      try {
          
          let sendMessage = '{"objects":[';
          for( const [key,value] of Object.entries(data[otherDevice]) ){
              sendMessage += JSON.stringify(value) + ',';
          }
          sendMessage = sendMessage.substr(0, sendMessage.length - 1) + ']}';
          server.send(sendMessage,senderInfo.port,senderInfo.address);
          
      } catch(e) {
          
      }

  
  } else if(message.includes('set')){
      
      let m = message.split('___');
      let deviceName = m[1];
      let array = JSON.parse(m[2]);
      
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
    console.log(data)
}, 1000) 

server.bind({
	address: 'localhost',
	port: 4000,
});