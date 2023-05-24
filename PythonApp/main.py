import cv2
from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.uix.image import Image
from kivy.graphics import Color
from kivy.graphics.texture import Texture
from kivy.clock import Clock
from roboflow import Roboflow

class MainApp(App):
    def build(self):
        self.layout = BoxLayout(orientation='vertical')
        # Set the background color to green
        with self.layout.canvas.before:
            Color(0, 1, 0, 1)  # Green color

        self.image = Image(
            pos_hint={'center_x': .5, 'center_y': .5},
            size_hint=(.5, .6),
            size=(300, 300)
        )
        self.layout.add_widget(self.image)

        self.start_button = Button(
            text="Start Video",
            pos_hint={'center_x': .5, 'center_y': .5},
            size_hint=(None, None),
            size=(200, 50)
        )
        self.start_button.bind(on_press=self.start_video)
        self.layout.add_widget(self.start_button)

        self.stop_button = Button(
            text="Stop Video",
            pos_hint={'center_x': .5, 'center_y': .5},
            size_hint=(None, None),
            size=(200, 50)
        )
        self.stop_button.bind(on_press=self.stop_video)
        self.layout.add_widget(self.stop_button)

        self.save_img_button = Button(
            text="Screenshot",
            pos_hint={'center_x': .5, 'center_y': .5},
            size_hint=(None, None),
            size=(200, 50)
        )
        self.save_img_button.bind(on_press=self.take_picture)
        self.layout.add_widget(self.save_img_button)

        self.capture = None
        self.image_frame = None
        self.predictions = None

        return self.layout

    def start_video(self, *args):
        self.capture = cv2.VideoCapture(1)
        Clock.schedule_interval(self.load_video, 1.0 / 60.0)

    def stop_video(self, *args):
        self.capture.release()
        Clock.unschedule(self.load_video)

    def load_video(self, *args):
        ret, frame = self.capture.read()

        buffer = cv2.flip(frame, 0).tostring()
        texture = Texture.create(size=(frame.shape[1], frame.shape[0]), colorfmt='bgr')
        texture.blit_buffer(buffer, colorfmt='bgr', bufferfmt='ubyte')

        self.image.texture = texture

        self.image_frame = frame

    def take_picture(self, *args):
        if self.image_frame is not None:
            image_name = "screenshot.png"

            buffer = cv2.flip(self.image_frame, 0).tostring()
            texture = Texture.create(size=(self.image_frame.shape[1], self.image_frame.shape[0]), colorfmt='bgr')
            texture.blit_buffer(buffer, colorfmt='bgr', bufferfmt='ubyte')

            # Infer on the camera frame
            predictions = model.predict(self.image_frame, confidence=10, overlap=50).json()

            for bounding_box in predictions['predictions']:
                x0 = bounding_box['x'] - bounding_box['width'] / 2
                x1 = bounding_box['x'] + bounding_box['width'] / 2
                y0 = bounding_box['y'] - bounding_box['height'] / 2
                y1 = bounding_box['y'] + bounding_box['height'] / 2

                start_point = (int(x0), int(y0))
                end_point = (int(x1), int(y1))
                cv2.rectangle(self.image_frame, start_point, end_point, color=(0, 255, 0), thickness=1)

                cv2.putText(
                    self.image_frame,
                    bounding_box["class"],
                    (int(x0), int(y0) - 10),
                    fontFace=cv2.FONT_HERSHEY_SIMPLEX,
                    fontScale=0.6,
                    color=(255, 0, 0),
                    thickness=2
                )

            # Display the frame with bounding boxes and class labels
            buffer = cv2.flip(self.image_frame, 0).tostring()
            texture.blit_buffer(buffer, colorfmt='bgr', bufferfmt='ubyte')
            self.image.texture = texture

            self.image_frame = self.image_frame

            cv2.imwrite(image_name, self.image_frame)

            # Stop the video capture and remove scheduled video loading
            self.capture.release()
            Clock.unschedule(self.load_video)

            # Display the last captured frame as an image
            self.image.texture = self.convert_frame_to_texture(self.image_frame)

    def convert_frame_to_texture(self, frame):
        buffer = cv2.flip(frame, 0).tostring()
        texture = Texture.create(size=(frame.shape[1], frame.shape[0]), colorfmt='bgr')
        texture.blit_buffer(buffer, colorfmt='bgr', bufferfmt='ubyte')
        return texture

if __name__ == '__main__':
    rf = Roboflow(api_key="J5vQlAYtrLQ3nesTH7I2")
    project = rf.workspace().project("coconut-leaf-pest-detection")
    model = project.version(13).model

    MainApp().run()