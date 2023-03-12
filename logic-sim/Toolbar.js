import Element from "./Element.js";

export default class Toolbar{
    constructor(width,canvasHeight,toolbarHeight,portSize) {
        this.elementTypes=[]
        this.elements=[]
        this.portSize = portSize
        this.height = canvasHeight;
        this.toolbarHeight = toolbarHeight
        this.width = width

        }
    addElement(type) {
        this.elementTypes.push(type)
        this.elements = []

        let dx = (this.width - 100) / this.elementTypes.length
        let center = dx * Math.floor(this.elementTypes.length / 2)
        if (this.elementTypes.length % 2 === 0)
            center = dx * (Math.floor(this.elementTypes.length / 2) - 0.5)
        let shift = this.width / 2 - center
        for (let i = 0; i < this.elementTypes.length; i++) {
            this.elements.push(new Element(this.elementTypes[i],
                'toolbar',
                {
                        x: dx * i + shift,
                        y: this.height - this.toolbarHeight / 2
                     },
                false,
                    this.portSize))
        }
    }

    getElementNearPos(pos) {
        return this.elements.reverse().find(e=>e.isPosInside(pos))
    }

}