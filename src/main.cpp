#include <Arduino.h>
#include <M5_Encoder.h>
#include <MicroOscSlip.h>

M5_Encoder myEncoder;
MicroOscSlip<128> monOsc(&Serial);

unsigned long monChronoDepart ;
void setup() {
  Wire.begin();
  Serial.begin(115200);
  myEncoder.begin();
}

void loop() { 
  if ( millis() - monChronoDepart >= 100 ){ //pour ralentir
  monChronoDepart = millis(); 
  myEncoder.update();
  int valeurEncodeur = myEncoder.getEncoderRotation();
  int changementEncodeur = myEncoder.getEncoderChange();
  int etatBouton = myEncoder.getButtonState();

  if (changementEncodeur > 0 ) {

    changementEncodeur = millis(); 
    myEncoder.setLEDColorRight( 30, 110, 255 );
    
  } else if ( changementEncodeur < 0 ) {

    myEncoder.setLEDColorRight( 0, 0, 0 );
    myEncoder.setLEDColorLeft( 30, 110, 255 );

  } else {
    myEncoder.setLEDColorLeft( 0, 0, 0 );
  }
  monOsc.sendInt( "/rota" , valeurEncodeur);
  monOsc.sendInt( "/bouton" , etatBouton);
}
}
