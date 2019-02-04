kubectl delete deployment tbwrk-deployment-prod
kubectl delete deployment tbwrk-deployment-dev
kubectl delete deployment tbwrk-deployment-test
kubectl delete deployment tbwrk-deployment-beta
kubectl create -f kubedep.yaml