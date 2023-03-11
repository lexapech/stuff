import LogicSim from "./LogicSim.js";

new LogicSim('#logic-sim-container',testCallback,false)

function testCallback(result){
    console.log(result)
}