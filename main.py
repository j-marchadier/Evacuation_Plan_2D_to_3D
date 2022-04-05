import detectPlanLegend

# Etape 1 => Detect plan and legend
detected = detectPlanLegend.detectPlanLegend("data/plans/plan-evacuation.jpg")

# Return a plan.jpg with only the plan
detected.findPlan()

# Return a legend.jpg with only the legend
detected.findLegend()





