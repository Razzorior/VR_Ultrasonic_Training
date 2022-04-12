let data = {};
let deviceNames = [];

const dgram = require('dgram');
const server = dgram.createSocket('udp4');

server.on('message', function(msg, senderInfo){
  //console.log('server got message:' + msg);
  let message = msg.toString();
    
  if(message.includes('get')) {
      let m = message.split('_');
      let deviceName = m[1];
      let name = deviceName === deviceNames[0] ? deviceName[1] : deviceName[0];
	  let sendMessage = '{"objects":[';//JSON.stringify(data);
      for( const [key,value] of Object.entries(data[name]) ){
          sendMessage += JSON.stringify(value) + ',';
      }
      sendMessage = sendMessage.substr(0, sendMessage.length - 1) + ']}';
	  server.send(sendMessage,senderInfo.port,senderInfo.address);
  
  } else if(message.includes('set')){
      
      let m = message.split('_');
      let deviceName = m[1];
      let array = JSON.parse(m[2]);
      
      if(!data[deviceName]){
          deviceNames.push(deviceName);
          data[deviceName] = {};
      }
      
      array.forEach( (obj) => {
          data[deviceName][obj.name] = obj;
      });
      
      console.log(data);
      
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