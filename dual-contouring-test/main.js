
function onload() {
    console.log("loaded")
    let canvas = document.querySelector("#canvas");
    let brush = document.querySelector('#draw')

    console.log(canvas)
    canvas.width=800;
    canvas.height=600;
    if(canvas.getContext("2d")){
        let ctx = canvas.getContext("2d")
        console.log(ctx)
        let grid=new Grid(20,20,ctx)
        brush.addEventListener("click",()=>{
            if(grid.changeBrush())
                brush.innerHTML="Добавить"
            else
            {
                brush.innerHTML="Удалить"
            }
        })
    }

}

class Grid {
    constructor(width,height,ctx) {
        this.ctx = ctx
        this.width = width;
        this.height = height;
        this.cellSizeX = (this.ctx.canvas.width)/this.width
        this.cellSizeY = (this.ctx.canvas.height)/this.height
        this.state=Array(this.width*this.height).fill(0)
        this.brush=true;
        this.activeCells=new Map()
        this.shift={x:this.cellSizeX/2,y:this.cellSizeY/2}
        this.ctx.translate(this.shift.x,this.shift.y)
        this.lastMousePos={};
        this.modificationRate=0.2;
        this.occupiedThreshold=0.1;
        this.addListeners();
        this.draw()
    }

    changeBrush() {
        this.brush=!this.brush
        return this.brush
    }

    addListeners() {
        this.ctx.canvas.addEventListener("mousedown", (e)=>this.onmousedown(e))
        this.ctx.canvas.addEventListener("mouseup", (e)=>this.onmouseup(e))
        this.ctx.canvas.addEventListener("mousemove", (e)=>this.onmousemove(e))
    }
    getCanvasCoordinates(pos) {
        let canvasBox = this.ctx.canvas.getBoundingClientRect()
        return {
            x: pos.x-canvasBox.x,
            y: pos.y-canvasBox.y,
        }
    }

    invertCell(pos) {

        let index=this.getIndexFromCell(pos)

        if(this.brush) {
            this.state[index]+=(1-this.state[index])*this.modificationRate
        }
        else {
            this.state[index]-=(this.state[index])*this.modificationRate
        }


        let edges = this.findEdges()
        this.draw()
        this.activeCells.clear()
        for(let i=0;i<edges.length;i++) {
            this.activateCells(edges[i])
            this.markEdge(edges[i])
        }
        this.drawActive()
        for(let cell of this.activeCells.values()){
           cell.pos = this.calculateCellPosition(cell)
        }
        for(let i=0;i<edges.length;i++) {
            this.connectActive(edges[i].cells)
        }

        //this.drawDot(this.getCanvasCoordinatesFromCell(pos),6,this.state[index])
    }

    getIndexFromCell(pos) {
        return pos.x+pos.y*this.width
    }

    getCellCoordinatesFromIndex(index){
        return {
            x:index % this.width,
            y:Math.floor(index/this.width)
        }
    }

    getCellCoordinates(pos){
        return {
            x: Math.round( (pos.x-this.shift.x)/this.cellSizeX),
            y: Math.round( (pos.y-this.shift.y)/this.cellSizeY)
        }
    }
    getCanvasCoordinatesFromCell(pos) {
        return {
            x:pos.x*this.cellSizeX,
            y:pos.y*this.cellSizeY
        }
    }

    isMouseMoved(pos) {
        let newPos = this.getCellCoordinates(pos)
        if( newPos.x !== this.lastMousePos.x || newPos.y !== this.lastMousePos.y){
            this.lastMousePos=newPos
            return true
        }
        return false
    }

    onmousemove(e){

        if(this.isMouseDown) {
            let pos=this.getCanvasCoordinates(e)
            if (true||this.isMouseMoved(pos))
                this.invertCell(this.getCellCoordinates(pos))
        }
    }
    onmousedown(e){
        this.isMouseDown=true
        let pos=this.getCanvasCoordinates(e)
        this.invertCell(this.getCellCoordinates(pos))
    }
    onmouseup(e){
        this.isMouseDown=false
        console.log("mouse up",this.getCanvasCoordinates(e))
    }
    drawDot(pos,size=4,filled) {
        this.ctx.fillStyle="#000"
        this.ctx.strokeStyle="#aaa"
        this.ctx.fillRect(pos.x-size/2*filled,pos.y-size/2*filled,size*filled,size*filled);
        /*if(filled)
            this.ctx.fillRect(pos.x-size/2,pos.y-size/2,size,size);
        else {
            this.ctx.fillStyle="#aaa"
            this.ctx.strokeRect(pos.x-size/2,pos.y-size/2,size,size);
        }*/

    }

    getOrAddCellToMap(key,edge) {
        let cell = this.activeCells.get(key)
        if(!cell) {
            let edges=[]
            edges.push(edge)
            this.activeCells.set(key,{ edges:edges})
            cell = this.activeCells.get(key)
        }
        else {
            cell.edges.push(edge)
        }
        return cell
    }

    getCellCenterFromIndex(index) {
        return {
            x: index % (this.width-1),
            y: Math.floor(index / (this.width-1))
        }
    }

    connectActive(cells){

        let pos1 = cells[0].pos
        let pos2 = cells[1].pos
        this.ctx.beginPath();
        this.ctx.moveTo(pos1.x, pos1.y);
        this.ctx.lineTo(pos2.x, pos2.y);
        this.ctx.lineWidth=1
        this.ctx.strokeStyle="#00f"
        this.ctx.stroke();
    }

    calculateCellPosition(cell) {
        let x=0
        let y=0
        for(let edge of cell.edges) {
             let edgeStartPos = this.getCanvasCoordinatesFromCell(this.getCellCoordinatesFromIndex(edge.cell))
              if(edge.direction) {
                  x+=edgeStartPos.x
                  y+=edgeStartPos.y+this.cellSizeY*edge.value
              }
              else {
                  x+=edgeStartPos.x+this.cellSizeX*edge.value
                  y+=edgeStartPos.y
              }
        }
        return {
            x: x / cell.edges.length,
            y: y / cell.edges.length
        }
    }


    activateCells(edge){
        let pos = this.getCellCoordinatesFromIndex(edge.cell)
        let key = pos.x+pos.y*(this.width-1)
        let cell=this.getOrAddCellToMap(key,edge)
        edge.cells.push(cell)
        if(edge.direction) {
            if(pos.x>0) {
                let key = pos.x-1+pos.y*(this.width-1)
                let cell=this.getOrAddCellToMap(key,edge)
                edge.cells.push(cell)
            }
        }
        else {
            if(pos.y>0) {
                let key=pos.x+(pos.y-1)*(this.width-1)
                let cell=this.getOrAddCellToMap(key,edge)
                edge.cells.push(cell)
            }
        }
    }


    markEdge(edge){
        let cellPos=this.getCellCoordinatesFromIndex(edge.cell)
        let pos1 = this.getCanvasCoordinatesFromCell(cellPos)
        let pos2;
        if(edge.direction) {
            pos2 = this.getCanvasCoordinatesFromCell({x:cellPos.x,y:cellPos.y+1})
        }
        else {
            pos2= this.getCanvasCoordinatesFromCell({x:cellPos.x+1,y:cellPos.y})
        }
        let dotPos={
            x: pos1.x+(pos2.x-pos1.x)*edge.value,
            y: pos1.y+(pos2.y-pos1.y)*edge.value
        }
        this.drawDot(dotPos,4,1)
        this.ctx.beginPath();
        this.ctx.moveTo(pos1.x, pos1.y);
        this.ctx.lineTo(pos2.x, pos2.y);
        this.ctx.lineWidth=1
        this.ctx.strokeStyle="#f00"
        this.ctx.stroke();
    }

    markCellActive(pos){
        let canvasPos=this.getCanvasCoordinatesFromCell(pos)
        this.ctx.fillStyle="#0f0"
        this.ctx.fillRect(canvasPos.x+this.cellSizeX/3,canvasPos.y+this.cellSizeY/3,this.cellSizeX/3,this.cellSizeY/3);
    }

    getStateAtPos(pos) {
        return this.state[this.getIndexFromCell(pos)]
    }

    isEdgeActive(pos1,pos2){
        if(pos2.x>=this.width || pos2.y>=this.height) return false
        return (this.getStateAtPos(pos1)<0.5 && this.getStateAtPos(pos2)>0.5) || (this.getStateAtPos(pos1)>0.5 && this.getStateAtPos(pos2)<0.5)
    }

    getEdgeValue(pos1,pos2){
        let vertex1 = this.getStateAtPos(pos1)
        let vertex2 = this.getStateAtPos(pos2)
        if(vertex1 < vertex2) {
            return 1.5-vertex1-vertex2
        }
        else {
            return -0.5+vertex1+vertex2
        }
    }

    findEdges() {
        let edges=[]
        for(let y=0;y<this.height;y++) {
            for(let x=0;x<this.width;x++) {
                let thisPos={x:x,y:y}
                let nextX={x:x+1,y:y}
                let nextY={x:x,y:y+1}
                if(this.isEdgeActive(thisPos,nextX))
                    edges.push({
                        cell: this.getIndexFromCell(thisPos),
                        direction: false,
                        cells: [],
                        value: this.getEdgeValue(thisPos,nextX)
                    })
                if(this.isEdgeActive(thisPos,nextY))
                    edges.push({
                        cell:this.getIndexFromCell(thisPos),
                        direction:true,
                        cells:[],
                        value: this.getEdgeValue(thisPos,nextY)
                    })
            }
        }
        return edges
    }

    drawActive(){
        for(let cell of this.activeCells.keys()) {
            this.markCellActive({x:cell%(this.width-1),y:Math.floor(cell/(this.width-1))})
        }


    }

    draw(){
        this.ctx.fillStyle="#fff"
        this.ctx.fillRect(-this.shift.x,-this.shift.y,800,600);
        this.ctx.fillStyle="#aaa"

        for(let x=0;x<this.width;x++) {
            this.ctx.fillRect(x*this.cellSizeX,0,1,600-this.cellSizeY);
        }
        for(let y=0;y<this.height;y++) {
            this.ctx.fillRect(0,y*this.cellSizeY,800-this.cellSizeX,1);
        }
        for(let y=0;y<this.height;y++) {
            for(let x=0;x<this.width;x++) {
                this.ctx.lineWidth=1
               this.drawDot({x:x*this.cellSizeX,y:y*this.cellSizeY},10,this.state[this.getIndexFromCell({x:x,y:y})])
            }
        }


    }
}


