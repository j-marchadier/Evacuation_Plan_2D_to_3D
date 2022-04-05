import detectPlanLegend
import Interface

#pathfile = Interface.SelectImage()
#print(pathfile)

# Etape 1 => Detect plan and legend
#detected = detectPlanLegend.detectPlanLegend(pathfile)
detected = detectPlanLegend.detectPlanLegend("data/plans/esiee.jpg")

# Return a legend.jpg with only the legend
#detected.findLegendAndPlan()

#detected.findLogos()

detected.DetectLogo()







