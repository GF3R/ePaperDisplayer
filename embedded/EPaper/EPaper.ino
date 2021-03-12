
#include <GxEPD.h>
#include <GxGDEW042T2/GxGDEW042T2.h> // 4.2" b/w
#include <GxIO/GxIO_SPI/GxIO_SPI.h>
#include <GxIO/GxIO.h>
#include <WiFi.h>
#include <PubSubClient.h>

const char *ssid = "Mainframe";
const char *pass = "**";
const char *mqtt_server = "farmer.cloudmqtt.com";
const char *mqtt_user = "prkyqurh";
const char *mqtt_pass = "**";
const int mqtt_port = 11140;
char *TOPIC = "epaper/";
char *SUB = "/image";
char *PUB = "data";
char buf[200];
char fullpubtopic[50];
char fullsubtopic[50];
char *combined;
WiFiClient wificlient;
PubSubClient client(wificlient);
char *DEVICEID = "EPAPER-001";

GxIO_Class io(SPI, /*CS=5*/ SS, /*DC=*/17, /*RST=*/16); // arbitrary selection of 17, 16
GxEPD_Class display(io, /*RST=*/16, /*BUSY=*/4);        // arbitrary selection of (16), 4

void setup()
{
  Serial.begin(115200);
  display.init(115200); // enable diagnostic output on Serial

  setupWifi();
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
  client.setBufferSize(1000000);
  strcpy(fullsubtopic, TOPIC);
  strcat(fullsubtopic, DEVICEID);
  strcat(fullsubtopic, SUB);

  strcpy(fullpubtopic, TOPIC);
  strcat(fullpubtopic, DEVICEID);
  Serial.println();
  Serial.println("setup done");
  while (!client.connected())
  {
    reconnect();
  }
  display.update();
}

void loop()
{

  if (WiFi.status() != WL_CONNECTED)
  {
    Serial.println("connection lost");
    WiFi.reconnect();
  }
  while (!client.connected())
  {
    reconnect();
  }
  client.loop();
}

void drawImage(const uint8_t *bytes)
{
  Serial.println("drawing image");
  uint16_t x = (display.width() - 64) / 2;
  uint16_t y = 5;
  display.fillScreen(GxEPD_WHITE);
  display.drawBitmap(bytes, x, y, 64, 180, GxEPD_WHITE);
  display.update();
}

void drawImageFullScreen(const uint8_t *bytes, int nofBytes)
{
  display.drawBitmap(bytes, nofBytes);
}

void clearScreen()
{
  display.fillScreen(GxEPD_WHITE);
  display.update();
}

void setupWifi()
{
  WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
}

////////////////////////////MQTT//////////////////////////////////

void callback(char *topic, byte *message, unsigned int length)
{
  Serial.println("Recieved message with length:");
  Serial.println(length);
  if (length == 0)
  {
    clearScreen();
    return;
  }
  drawImageFullScreen(message, length);
}

void publish(char *message)
{
  Serial.println(fullpubtopic);
  client.publish(fullpubtopic, message);
}

void reconnect()
{
  while (!client.connected())
  {
    Serial.print("Attempting MQTT connection...");
    if (client.connect(DEVICEID, mqtt_user, mqtt_pass))
    {
      Serial.println("connected");
      client.subscribe(fullsubtopic);
      Serial.println(fullsubtopic);
    }
    else
    {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}
//^^^^^^^^^^^^^^^^^^^^^^^^^^MQTT^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
