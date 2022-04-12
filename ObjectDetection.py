import cv2
import numpy as np
import glob

import CreateXML

DEFAULT_TEMPLATE_MATCHING_THRESHOLD = 0.5


def compute_iou(boxA, boxB):
    # determine the (x, y)-coordinates of the intersection rectangle
    xA = max(boxA["TOP_LEFT_X"], boxB["TOP_LEFT_X"])
    yA = max(boxA["TOP_LEFT_Y"], boxB["TOP_LEFT_Y"])
    xB = min(boxA["BOTTOM_RIGHT_X"], boxB["BOTTOM_RIGHT_X"])
    yB = min(boxA["BOTTOM_RIGHT_Y"], boxB["BOTTOM_RIGHT_Y"])
    # compute the area of intersection rectangle
    interArea = max(0, xB - xA + 1) * max(0, yB - yA + 1)
    # compute the area of both the prediction and ground-truth
    # rectangles
    boxAArea = (boxA["BOTTOM_RIGHT_X"] - boxA["TOP_LEFT_X"] + 1) * (boxA["BOTTOM_RIGHT_Y"] - boxA["TOP_LEFT_Y"] + 1)
    boxBArea = (boxB["BOTTOM_RIGHT_X"] - boxB["TOP_LEFT_X"] + 1) * (boxB["BOTTOM_RIGHT_Y"] - boxB["TOP_LEFT_Y"] + 1)
    # compute the intersection over union by taking the intersection
    # area and dividing it by the sum of prediction + ground-truth
    # areas - the interesection area
    iou = interArea / float(boxAArea + boxBArea - interArea)
    # return the intersection over union value
    return iou


def non_max_suppression(
        objects,
        non_max_suppression_threshold=0.5,
        score_key="MATCH_VALUE",
):
    """
    Filter objects overlapping with IoU over threshold by keeping only the one with maximum score.
    Args:
        objects (List[dict]): a list of objects dictionaries, with:
            {score_key} (float): the object score
            {top_left_x} (float): the top-left x-axis coordinate of the object bounding box
            {top_left_y} (float): the top-left y-axis coordinate of the object bounding box
            {bottom_right_x} (float): the bottom-right x-axis coordinate of the object bounding box
            {bottom_right_y} (float): the bottom-right y-axis coordinate of the object bounding box
        non_max_suppression_threshold (float): the minimum IoU value used to filter overlapping boxes when
            conducting non max suppression.
        score_key (str): score key in objects dicts
    Returns:
        List[dict]: the filtered list of dictionaries.
    """
    sorted_objects = sorted(objects, key=lambda obj: obj[score_key], reverse=True)
    filtered_objects = []
    for object_ in sorted_objects:
        overlap_found = False
        for filtered_object in filtered_objects:
            iou = compute_iou(object_, filtered_object)
            if iou > non_max_suppression_threshold:
                overlap_found = True
                break
        if not overlap_found:
            filtered_objects.append(object_)
    return filtered_objects


class Template:
    """
    A class defining a template
    """

    def __init__(self, image_path, label, matching_threshold=DEFAULT_TEMPLATE_MATCHING_THRESHOLD):
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
        self.template = cv2.imread(image_path)
        self.template_height, self.template_width = self.template.shape[:2]
        self.matching_threshold = matching_threshold


def main(imgPath=None):
    if imgPath is None: imgPath = "data/plans/plan.jpg"
    image = cv2.imread(imgPath)

    label_logos = CreateXML.readLabelXML("data/plans/legend.xml")
    allFiles = glob.glob("data/logos/*.jpg")
    print(label_logos)
    templates = []
    for logo,label in zip(allFiles,label_logos):
        templates.append(Template(image_path=logo,
                                  label=label,))

    #templates = [
        #Template(image_path="data/logos/logo_0.jpg", label="1", color=(0, 0, 0)),
        #Template(image_path="data/logos/logo_2.jpg", label="2", color=(0, 255, 0)),
        #Template(image_path="data/logos/logo_1.jpg", label="3", color=(0, 0, 100)),
    #]

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
            }

            detections.append(match)

    image_with_detections = image.copy()

    NMS_THRESHOLD = 0.2
    detections = non_max_suppression(detections, non_max_suppression_threshold=NMS_THRESHOLD)

    for detection in detections:
        cv2.rectangle(
            image_with_detections,
            (detection["TOP_LEFT_X"], detection["TOP_LEFT_Y"]),
            (detection["BOTTOM_RIGHT_X"], detection["BOTTOM_RIGHT_Y"]),
            (255,255,255),
            -1,
        )
        #cv2.putText(
            #image_with_detections,
            #f"{detection['LABEL']} - {detection['MATCH_VALUE']}",
            #(detection["TOP_LEFT_X"] + 2, detection["TOP_LEFT_Y"] + 20),
            #cv2.FONT_HERSHEY_SIMPLEX,
            #1.0,
            #(0,0,0),
            #1,
            #cv2.LINE_AA,
        #)

    cv2.imwrite(f"data/result.jpg", image_with_detections)


