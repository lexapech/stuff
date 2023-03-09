
export default class Element{
    constructor(type,name,pos,draggable) {
        this.name = name
        this.type = type
        this.pos = pos
        this.draggable = draggable
    }

    initPortBar(type,width,height,ports){
        this.draggable = false;
        this.type = 'bar'
        this.name = type+'_bar'
        this.width = width
        this.portType = type
        this.height = height
        this.portCount = ports
    }

    getSize(){
        if(this.type==='not')
            return {width: 160, height: 40}
        else if(this.type==='bar')
            return {width: this.width,height: this.height}
        else {
            return {width: 160, height: 80}
        }
    }

    canDrag(pos) {
        if(this.draggable!==true) return false;
        let size = this.getSize()
        let tl = {x:this.pos.x - size.width/2,y:this.pos.y-size.height/2}
        let br = {x:this.pos.x + size.width/2,y:this.pos.y+size.height/2}
        return (pos.x >= tl.x && pos.x < br.x) && (pos.y >= tl.y && pos.y < br.y)
    }

    getInputPortCount() {
        if(this.type==='not')
            return 1;
        else if(this.type==='bar')
            return this.portCount
        else {
            return 2;
        }
    }
    getInputPortsPos(){
        if(this.type==='bar' && (this.portType==='output' || this.portType==='common')) return []
        if(this.type==='bar' && (this.portType==='input' )) {
            let dy = this.height / this.portCount
            let center = dy * Math.floor(this.portCount / 2)
            if (this.portCount % 2 === 0)
                center = dy * (Math.floor(this.portCount / 2) - 0.5)
            let shift = this.height / 2 - center
            let ports = []


            for (let i=0;i<this.portCount;i++){
                ports.push({x: this.pos.x - this.width / 2, y: this.pos.y - this.height / 2 + i*dy +shift})
            }
            return ports
        }
        else {
            let iPorts = this.getInputPortCount()
            let elementSize = this.getSize()
            if (iPorts === 1) {
                return [{x: this.pos.x - elementSize.width / 2, y: this.pos.y}]
            } else {
                return [
                    {x: this.pos.x - elementSize.width / 2, y: this.pos.y - elementSize.height / 4},
                    {x: this.pos.x - elementSize.width / 2, y: this.pos.y + elementSize.height / 4}
                ]
            }
        }
    }

    getOutputPortsPos(){
        if(this.type==='bar' && (this.portType==='input')) return []
        if(this.type==='bar' && (this.portType==='output' || this.portType==='common')) {
            let dy = this.height / this.portCount
            let center = dy * Math.floor(this.portCount / 2)
            if (this.portCount % 2 === 0)
                center = dy * (Math.floor(this.portCount / 2) - 0.5)
            let shift = this.height / 2 - center
            let ports = []
            let output = 1
            if(this.portType==='common') output = 0
            for (let i=0;i<this.portCount;i++){
                ports.push({x: this.pos.x + output*(this.width / 2), y: this.pos.y - this.height / 2 + i*dy +shift})
            }
            return ports
        }
        let elementSize = this.getSize()
        return [{x: this.pos.x+elementSize.width/2,y: this.pos.y}]
    }
}