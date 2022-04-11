let data = {};

const dgram = require('dgram');
const server = dgram.createSocket('udp4');

server.on('message', function(msg, senderInfo){
  console.log('server got message:' + msg);
  let message = msg.toString();
    
  if(message.includes('get')) {
      
	  let sendMessage = '';
	  server.send(sendMessage,senderInfo.port,senderInfo.address);
  
  } else if(message.includes('set')){
      
      let m = message.split('_');
      let deviceName = m[1];
      let retrievedObject = JSON.parse(m[2]);
      
      if(!data[deviceName]){
          data[deviceName] = {};
      }
      
      data[deviceName][retrievedObject.name] = retrievedObject;
      
  }
  
});

server.on('listening', function(){
	const address = server.address();
	console.log('server is listening on ' + address.address + ':' + address.port);
});

server.bind({
	address: 'localhost',
	port: 4000,
});