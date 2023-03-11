export default class Port {
    constructor(element, type, id, size) {
        this.element = element;
        this.type = type;
        this.id = id;
        this.pos = undefined
        this.size = size
    }

    getType(editor){
        if(this.type === 'common'){
            if(editor) return 'input'
            else return 'output'
        }
        return this.type
    }

    getPos(){
        if(this.type === "mouse") return this.pos;
        let portCount = this.element.getPortCount();
        let elementSize = this.element.getSize();
        let typedPortCount = portCount.input;
        let x = 0;
        if(this.type==='input') {
            x = -elementSize.width / 2;
        }
        else if(this.type==='output'){
            typedPortCount=portCount.output;
            x = elementSize.width / 2;
        }
        let dy = elementSize.height / typedPortCount;
        let center = dy * Math.floor(typedPortCount / 2);
        if (typedPortCount % 2 === 0)
            center = dy * (Math.floor(typedPortCount / 2) - 0.5);
        let shift = elementSize.height / 2 - center;
        return {x: this.element.pos.x + x,
                y: this.element.pos.y - elementSize.height / 2 + this.id * dy + shift};
    }

}