pipeline {
  agent any
  environment {
    DEV_VERSION = 'latest'
	APP_VERSION = '2.0'
	HUB_NAME = 'xlnt_auth-api'
  }
  stages {
	stage('Build Stage') {
	  steps {
	   script {
			if (GIT_BRANCH == 'build') {
				sh '''docker build . -t ${XLNT_HUB1}/${HUB_NAME}:${APP_VERSION}'''
			} 
			if (GIT_BRANCH == 'master') {
				sh '''docker build . -t ${XLNT_HUB2}/${HUB_NAME}:${DEV_VERSION}'''
			}
		}	
	  }
	}
	stage('Push Stage') {
	  steps {
	   script {
			if (GIT_BRANCH == 'build') {
				sh '''docker push ${XLNT_HUB1}/${HUB_NAME}:${APP_VERSION}'''
			} 
			if (GIT_BRANCH == 'master') {
				sh '''docker push ${XLNT_HUB2}/${HUB_NAME}:${DEV_VERSION}'''
			}
		}
	  }
	}
	stage('Deploy Stage') {
	  steps {
	   script {
			if (GIT_BRANCH == 'build') {
				build wait: false, job: 'Pipeline Deploy 144'
			} 
			if (GIT_BRANCH == 'master') {
				build wait: false, job: 'Pipeline Deploy 145'
			}
		}
	  }
	}

  }
  
}