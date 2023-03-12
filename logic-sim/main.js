import LogicSim from "./LogicSim.js";

new LogicSim('#logic-sim-container',0,testCallback,false)

function testCallback(result){
    console.log(result)
}