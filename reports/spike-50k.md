# Rapport — Spike test 50k

**Test exécuté** : `task spike-50k` (spike test, 50 000 films)

## 1. Capture Grafana

_Collez ici une capture d’écran du dashboard Grafana (http://localhost:3000/d/k6-load-testing/k6-load-testing) pendant ou après l’exécution du test._

<!-- Remplacer par votre capture, ex. : ![Capture spike-50k](captures/spike-50k.png) -->

![Capture spike-50k](captures/spike-50k.png)

## 2. Observations

- Le pic soudain provoque une dégradation sévère : 50% d'erreurs et latences qui s'envolent à 1,22 s sur seulement 4 requêtes.
