import detectPlanLegend
import Interface
import CreateXML

pathfile = Interface.SelectImage()
print(pathfile)

# Etape 1 => Detect plan and legend
detected = detectPlanLegend.detectPlanLegend(pathfile)

# Return a legend.jpg with only the legend
#detected.findLegendAndPlan()

detected.findLogos()







