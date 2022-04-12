import cv2
import numpy as np

DEFAULT_TEMPLATE_MATCHING_THRESHOLD = 0.5


class Template:
    """
    A class defining a template
    """
    def __init__(self, image_path, label, color, matching_threshold=DEFAULT_TEMPLATE_MATCHING_THRESHOLD):
        """
        Args:
            image_path (str): path of the template image path
            label (str): the label corresponding to the template
            color (List[int]): the color associated with the label (to plot detections)
            matching_threshold (float): the minimum similarity score to consider an object is detected by template
                matching
        """
        self.image_path = image_path
        self.label = label
        self.color = color
        self.template = cv2.imread(image_path)
        self.template_height, self.template_width = self.template.shape[:2]
        self.matching_threshold = matching_threshold


image = cv2.imread("data/otherPlans/Capture d’écran 2022-04-12 à 14.25.26.png")


templates = [
    Template(image_path="Capture d’écran 2022-04-12 à 14.22.42.png", label="1", color=(0, 0, 255)),
    Template(image_path="Capture d’écran 2022-04-12 à 14.22.42.png", label="2", color=(0, 255, 0)),
    Template(image_path="Capture d’écran 2022-04-12 à 14.26.53.png", label="3", color=(0, 191, 255)),
]

detections = []
for template in templates:
    template_matching = cv2.matchTemplate(
        template.template, image, cv2.TM_CCOEFF_NORMED
    )

    match_locations = np.where(template_matching >= template.matching_threshold)

    for (x, y) in zip(match_locations[1], match_locations[0]):
        match = {
            "TOP_LEFT_X": x,
            "TOP_LEFT_Y": y,
            "BOTTOM_RIGHT_X": x + template.template_width,
            "BOTTOM_RIGHT_Y": y + template.template_height,
            "MATCH_VALUE": template_matching[y, x],
            "LABEL": template.label,
            "COLOR": template.color
        }

        detections.append(match)


image_with_detections = image.copy()

print(detections)


for detection in detections:
    cv2.rectangle(
        image_with_detections,
        (detection["TOP_LEFT_X"], detection["TOP_LEFT_Y"]),
        (detection["BOTTOM_RIGHT_X"], detection["BOTTOM_RIGHT_Y"]),
        detection["COLOR"],
        2,
    )
    cv2.putText(
        image_with_detections,
        f"{detection['LABEL']} - {detection['MATCH_VALUE']}",
        (detection["TOP_LEFT_X"] + 2, detection["TOP_LEFT_Y"] + 20),
        cv2.FONT_HERSHEY_SIMPLEX,
        0.5,
        detection["COLOR"],
        1,
        cv2.LINE_AA,
    )

cv2.imwrite(f"result.jpeg", image_with_detections)