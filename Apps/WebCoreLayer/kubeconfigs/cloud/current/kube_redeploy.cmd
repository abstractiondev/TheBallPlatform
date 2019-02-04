kubectl delete deployment tbweb-deployment-prod
kubectl delete deployment tbweb-deployment-dev
kubectl delete deployment tbweb-deployment-test
kubectl delete deployment tbweb-deployment-beta
kubectl create -f kubedep.yaml