(function(){dust.register("labelhiddeninput_singleline.dust",body_0);function body_0(chk,ctx){return chk.write("<div class=\"control-group\"><div class=\"controls\"><label class=\"control-label\" name=\"").reference(ctx.get("input_name"),ctx,"h").write("\" ").exists(ctx.get("label_id"),ctx,{"block":body_1},null).write(" >").exists(ctx.get("input_value"),ctx,{"block":body_2},null).write("</label><input type=\"hidden\" name=\"").reference(ctx.get("input_name"),ctx,"h").write("\" ").exists(ctx.get("input_value"),ctx,{"block":body_3},null).write(" /></div></div>");}function body_1(chk,ctx){return chk.write("id=\"").reference(ctx.get("label_id"),ctx,"h").write("\"");}function body_2(chk,ctx){return chk.reference(ctx.get("input_value"),ctx,"h");}function body_3(chk,ctx){return chk.write("value=\"").reference(ctx.get("input_value"),ctx,"h").write("\"");}return body_0;})();