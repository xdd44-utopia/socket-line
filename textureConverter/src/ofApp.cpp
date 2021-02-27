#include "ofApp.h"
#include <string>
#include <fstream>

//--------------------------------------------------------------
void ofApp::setup(){
	ofstream ofile;
	ofile.open("/Users/xdd44/Documents/Creation/SCM/socket-line/textureConverter/bin/data/colors.txt", ofstream::out);
	for (int i=0;i<N;i++) {
		string name = "pngHead/cabeza" + to_string(i) + ".png";
		images[i].load(name);
		images[i].setImageType(OF_IMAGE_COLOR_ALPHA);
		matImages[i] = toCv(images[i]);
		cout << i << endl;
		for (int j=0;j<SIZE;j++) {
			for (int k=0;k<SIZE;k++){
				ofile << setprecision(3);
				ofile << fixed;
				ofile << matImages[i].at<Vec4b>(j, k) << endl;
				if (i == 10) {
					cout << matImages[i].at<Vec4b>(j, k) << endl;
				}
			}
		}
	}
	ofile.close();
}

//--------------------------------------------------------------
void ofApp::update(){
	
}

//--------------------------------------------------------------
void ofApp::draw(){
	ofBackground(255);
	drawMat(matImages[counter], 0, 0);
	counter++;
	counter = (counter == N ? 0 : counter);
}

//--------------------------------------------------------------
void ofApp::keyPressed(int key){

}

//--------------------------------------------------------------
void ofApp::keyReleased(int key){

}

//--------------------------------------------------------------
void ofApp::mouseMoved(int x, int y ){

}

//--------------------------------------------------------------
void ofApp::mouseDragged(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mousePressed(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseReleased(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseEntered(int x, int y){

}

//--------------------------------------------------------------
void ofApp::mouseExited(int x, int y){

}

//--------------------------------------------------------------
void ofApp::windowResized(int w, int h){

}

//--------------------------------------------------------------
void ofApp::gotMessage(ofMessage msg){

}

//--------------------------------------------------------------
void ofApp::dragEvent(ofDragInfo dragInfo){ 

}
