import LogicSim from "./LogicSim.js";

new LogicSim(document.querySelector("#logic-sim-container"),0,testCallback,false)

function testCallback(result){
    console.log(result)
}