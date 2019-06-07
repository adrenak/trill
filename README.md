# Trill
Data transmission using sound waves

Trill is a Unity library for sending and receiving data using sound waves. The idea originally came from [Chirp](https://www.chirp.io). Initially, a rough version was made for a client of mine but transferred data at a much slower speed. This project was started to make a better version.

## API
To use Trill, you mainly have to work with `TrillDetector` and `TrillEmitter`

`TrillEmitter`  
Allows you to pass data in the form of a string or byte array. The emitter converts the data into a series of frequencies that can be emitted.
Sample usage:
```
TrillEmitter m_Emitter = TrillEmitter.New();
m_Emitter.EmitString(input);
```


`TrillDetector`  
Listens for the emitter's sound and when a full message is detected it fires an event with the byte array representing the message.

Sample usage:
```
TrillDetector d = TrillDetector.New();
d.StartDetecting();
d.OnGetMessage += bytes => {
    // Do something with the bytes received
};
```

`TrillK`
This class allows the important parameters of the library to be assigned. Default values will suffice most of the time.

## Future work
Presently Trill has low data transfer rate (2 bytes/second) and often produces errors in detecting the messages. Error correction algorithms and faster transfer rates are required.

## Contact
[@github](https://www.github.com/adrenak)  
[@www](http://www.vatsalambastha.com)